using eShop.Auth.Api.Messages.Email;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public sealed record LoginCommand(LoginRequest Request) : IRequest<Result>;

public sealed class LoginCommandHandler(
    ITokenManager tokenManager,
    IUserManager userManager,
    ILockoutManager lockoutManager,
    ICodeManager codeManager,
    IMessageService messageService,
    IReasonManager reasonManager) : IRequestHandler<LoginCommand, Result>
{
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly IUserManager userManager = userManager;
    private readonly ILockoutManager lockoutManager = lockoutManager;
    private readonly ICodeManager codeManager = codeManager;
    private readonly IMessageService messageService = messageService;
    private readonly IReasonManager reasonManager = reasonManager;
    private const int MaxLoginAttempts = 5;

    public async Task<Result> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Request.Email, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with email {request.Request.Email}.");
        }

        if (!user.EmailConfirmed)
        {
            return Results.BadRequest("The email address is not confirmed.");
        }

        var lockoutState = await lockoutManager.FindAsync(user, cancellationToken);

        if (lockoutState.Enabled)
        {
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

            if (user.FailedLoginAttempts < MaxLoginAttempts)
            {
                return Results.BadRequest("The password is not valid.",
                    new LoginResponse()
                    {
                        FailedLoginAttempts = user.FailedLoginAttempts,
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

            var code = await codeManager.GenerateAsync(user, SenderType.Email, 
                CodeType.Recover, CodeResource.Account, cancellationToken);

            var message = new AccountRecoveryMessage()
            {
                Credentials = new()
                {
                    { "To", user.Email },
                    { "Subject", "Account recovery" }
                },
                Payload = new()
                {
                    { "UserName", user.UserName },
                    { "Code", code },
                }
            };

            await messageService.SendMessageAsync(SenderType.Email, message, cancellationToken);

            return Results.BadRequest("Account is locked out due to too many failed login attempts",
                new LoginResponse()
                {
                    UserId = user.Id,
                    IsLockedOut = true,
                    FailedLoginAttempts = user.FailedLoginAttempts,
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

        if (user.TwoFactorEnabled)
        {
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
            TwoFactorEnabled = user.TwoFactorEnabled,
            IsLockedOut = lockoutState.Enabled,
        };
        
        return Result.Success(response, "Successfully logged in.");
    }
}