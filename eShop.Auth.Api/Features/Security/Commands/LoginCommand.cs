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

        var emailResult = await CheckEmailAsync(user, request.Context, cancellationToken);
        if(!emailResult.Succeeded) return emailResult;

        var lockoutState = await lockoutManager.FindAsync(user, cancellationToken);

        if (lockoutState.Enabled)
        {
            return await LockedOutAsync(user, lockoutState, request.Context, cancellationToken);
        }

        var validationResult = await ValidatePasswordAsync(user, request.Request.Password, request.Context, cancellationToken);
        if (!validationResult.Succeeded) return validationResult;

        if (user.FailedLoginAttempts > 0)
        {
            var resetResult = await ResetFailedLoginAttemptsAsync(user, cancellationToken);
            if (!resetResult.Succeeded) return resetResult;
        }
        
        if (user.Providers.Any(x => x.Subscribed))
        {
            return await CheckTwoFactorStateAsync(user, lockoutState, request.Context, cancellationToken);
        }

        var tokenResult = await GenerateTokensAsync(user, lockoutState, request.Context, cancellationToken);
        return tokenResult;
    }

    private async Task<Result> LockedOutAsync(UserEntity user, LockoutStateEntity lockoutState, 
        HttpContext context, CancellationToken cancellationToken)
    {
        var loginSessionResult = await loginSessionManager.CreateAsync(user, context, 
            LoginStatus.Locked, LoginType.Password, cancellationToken: cancellationToken);
            
        if (!loginSessionResult.Succeeded)
        {
            return loginSessionResult;
        }
            
        return Results.BadRequest("Account is locked out", new LoginResponse()
        {
            UserId = user.Id,
            IsLockedOut = lockoutState.Enabled,
            Reason = Mapper.Map(lockoutState.Reason),
        });
    }

    private async Task<Result> CheckEmailAsync(UserEntity user, HttpContext context, CancellationToken cancellationToken)
    {
        if (identityOptions.SignIn.RequireConfirmedEmail && !user.EmailConfirmed)
        {
            var result = await loginSessionManager.CreateAsync(user, context, 
                LoginStatus.Failed, LoginType.Password, cancellationToken: cancellationToken);
            
            if (!result.Succeeded)
            {
                return result;
            }
            
            return Results.BadRequest("The email address is not confirmed.");
        }
        
        return Result.Success();
    }

    private async Task<Result> ValidatePasswordAsync(UserEntity user, string password, 
        HttpContext context, CancellationToken cancellationToken)
    {
        var isValidPassword = await userManager.CheckPasswordAsync(user, password, cancellationToken);

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
                var loginSessionResult = await loginSessionManager.CreateAsync(user, context, 
                    LoginStatus.Failed, LoginType.Password, cancellationToken: cancellationToken);
                
                if (!loginSessionResult.Succeeded)
                {
                    return loginSessionResult;
                }
                
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

            var result = await loginSessionManager.CreateAsync(user, context, 
                LoginStatus.Locked, LoginType.Password, cancellationToken: cancellationToken);
            
            if (!result.Succeeded)
            {
                return result;
            }
            
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
        
        return Result.Success();
    }

    private async Task<Result> ResetFailedLoginAttemptsAsync(UserEntity user, CancellationToken cancellationToken)
    {
        user.FailedLoginAttempts = 0;
        var result = await userManager.UpdateAsync(user, cancellationToken);
        return result;
    }
    
    private async Task<Result> CheckTwoFactorStateAsync(UserEntity user, LockoutStateEntity lockoutState, 
        HttpContext context, CancellationToken cancellationToken)
    {
        var result = await loginSessionManager.CreateAsync(user, context, 
            LoginStatus.PendingMfa, LoginType.Password, cancellationToken: cancellationToken);
            
        if (!result.Succeeded)
        {
            return result;
        }
            
        return Result.Success(new LoginResponse()
        {
            IsLockedOut = lockoutState.Enabled,
            TwoFactorEnabled = true,
            UserId = user.Id
        });
    }
    
    private async Task<Result> GenerateTokensAsync(UserEntity user, LockoutStateEntity lockoutState, 
        HttpContext context, CancellationToken cancellationToken)
    {
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
        
        var result = await loginSessionManager.CreateAsync(user, context, 
            LoginStatus.Success, LoginType.Password, cancellationToken: cancellationToken);
        
        if (!result.Succeeded)
        {
            return result;
        }
        
        return Result.Success(response, "Successfully logged in.");
    }
}