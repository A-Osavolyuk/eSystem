using eShop.Auth.Api.Messages.Email;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public sealed record LoginCommand(LoginRequest Request, HttpContext Context) : IRequest<Result>;

public sealed class LoginCommandHandler(
    ITokenManager tokenManager,
    IUserManager userManager,
    ILockoutManager lockoutManager,
    IReasonManager reasonManager,
    ILoginSessionManager loginSessionManager,
    IdentityOptions identityOptions) : IRequestHandler<LoginCommand, Result>
{
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly IUserManager userManager = userManager;
    private readonly ILockoutManager lockoutManager = lockoutManager;
    private readonly IReasonManager reasonManager = reasonManager;
    private readonly ILoginSessionManager loginSessionManager = loginSessionManager;
    private readonly IdentityOptions identityOptions = identityOptions;
    public async Task<Result> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        UserEntity? user = null;
        
        if (identityOptions.SignIn.AllowUserNameLogin)
        {
            user = await userManager.FindByUserNameAsync(request.Request.Login, cancellationToken);
        }

        if (user is null && identityOptions.SignIn.AllowEmailLogin)
        {
            user = await userManager.FindByEmailAsync(request.Request.Login, cancellationToken);
        }

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with login {request.Request.Login}.");
        }

        if (identityOptions.SignIn.RequireConfirmedEmail && !user.EmailConfirmed)
        {
            await loginSessionManager.CreateAsync(user, request.Context, 
                LoginStatus.Failed, LoginType.Password, cancellationToken: cancellationToken);
            
            return Results.BadRequest("The email address is not confirmed.");
        }

        var lockoutState = await lockoutManager.FindAsync(user, cancellationToken);

        if (lockoutState.Enabled)
        {
            await loginSessionManager.CreateAsync(user, request.Context, 
                LoginStatus.Locked, LoginType.Password, cancellationToken: cancellationToken);
            
            return Results.BadRequest("Account is locked out", new LoginResponse()
            {
                UserId = user.Id,
                IsLockedOut = lockoutState.Enabled,
                Reason = Mapper.Map(lockoutState.Reason),
            });
        }

        var isValidPassword = await userManager.CheckPasswordAsync(user, request.Request.Password, cancellationToken);

        if (!isValidPassword)
        {
            user.FailedLoginAttempts += 1;

            var updateResult = await userManager.UpdateAsync(user, cancellationToken);

            if (!updateResult.Succeeded)
            {
                return updateResult;
            }

            if (user.FailedLoginAttempts < identityOptions.SignIn.MaxFailedLoginAttempts)
            {
                await loginSessionManager.CreateAsync(user, request.Context, 
                    LoginStatus.Failed, LoginType.Password, cancellationToken: cancellationToken);
                
                return Results.BadRequest("The password is not valid.",
                    new LoginResponse()
                    {
                        FailedLoginAttempts = user.FailedLoginAttempts,
                        MaxFailedLoginAttempts = identityOptions.SignIn.MaxFailedLoginAttempts,
                        IsLockedOut = false,
                        UserId = user.Id,
                    });
            }

            var reason = await reasonManager.FindByTypeAsync(LockoutType.TooManyFailedLoginAttempts, cancellationToken);

            if (reason is null)
            {
                return Results.NotFound($"Cannot find lockout type {LockoutType.TooManyFailedLoginAttempts}.");
            }
            
            var lockoutResult = await lockoutManager.LockoutAsync(user, reason, permanent: true, cancellationToken: cancellationToken);

            if (!lockoutResult.Succeeded)
            {
                return lockoutResult;
            }

            await loginSessionManager.CreateAsync(user, request.Context, 
                LoginStatus.Locked, LoginType.Password, cancellationToken: cancellationToken);
            
            return Results.BadRequest("Account is locked out due to too many failed login attempts",
                new LoginResponse()
                {
                    UserId = user.Id,
                    IsLockedOut = true,
                    FailedLoginAttempts = user.FailedLoginAttempts,
                    MaxFailedLoginAttempts = identityOptions.SignIn.MaxFailedLoginAttempts,
                    Reason = Mapper.Map(reason)
                });
        }

        if (user.FailedLoginAttempts > 0)
        {
            user.FailedLoginAttempts = 0;
            var updateResult = await userManager.UpdateAsync(user, cancellationToken);

            if (!updateResult.Succeeded)
            {
                return updateResult;
            }
        }

        if (user.Providers.Any(x => x.Subscribed))
        {
            await loginSessionManager.CreateAsync(user, request.Context, 
                LoginStatus.PendingMfa, LoginType.Password, cancellationToken: cancellationToken);
            
            return Result.Success(new LoginResponse()
            {
                IsLockedOut = lockoutState.Enabled,
                TwoFactorEnabled = true,
                UserId = user.Id
            });
        }

        var accessToken = await tokenManager.GenerateAsync(user, TokenType.Access, cancellationToken);
        var refreshToken = await tokenManager.GenerateAsync(user, TokenType.Refresh, cancellationToken);

        var response = new LoginResponse()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            UserId = user.Id,
            TwoFactorEnabled = user.Providers.Any(x => x.Subscribed),
            IsLockedOut = lockoutState.Enabled,
        };
        
        await loginSessionManager.CreateAsync(user, request.Context, 
            LoginStatus.Success, LoginType.Password, cancellationToken: cancellationToken);
        
        return Result.Success(response, "Successfully logged in.");
    }
}