using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record LoginCommand(LoginRequest Request) : IRequest<Result>;

internal sealed class LoginCommandHandler(
    ITokenManager tokenManager,
    IUserManager userManager,
    ILockoutManager lockoutManager,
    ICodeManager codeManager,
    IMessageService messageService) : IRequestHandler<LoginCommand, Result>
{
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly IUserManager userManager = userManager;
    private readonly ILockoutManager lockoutManager = lockoutManager;
    private readonly ICodeManager codeManager = codeManager;
    private readonly IMessageService messageService = messageService;
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

        if (lockoutState.IsActive)
        {
            return Results.BadRequest("Account is locked out", new LoginResponse()
            {
                UserId = user.Id,
                IsLockedOut = lockoutState.IsActive,
                Reason = lockoutState.Reason,
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

            if (user.FailedLoginAttempts >= MaxLoginAttempts)
            {
                if (!lockoutState.IsActive)
                {
                    var lockoutResult = await lockoutManager.LockoutAsync(user, LockoutReason.TooManyFailedLoginAttempts,
                        "Too many failed login attempts", LockoutPeriod.Permanent, cancellationToken: cancellationToken);

                    if (!lockoutResult.Succeeded)
                    {
                        return lockoutResult;
                    }
                    
                    var code = await codeManager.GenerateAsync(user, CodeType.Recover, cancellationToken);
                    var payload = new { Code = code };
                    var credentials = new EmailCredentials()
                    {
                        Subject = "Account Recovery",
                        To = user.Email,
                        UserName = user.UserName
                    };
                
                    await messageService.SendMessageAsync(SenderType.Email, "account-recovery",
                        payload, credentials, cancellationToken);
                }

                var response = new LoginResponse()
                {
                    FailedLoginAttempts = user.FailedLoginAttempts,
                    IsLockedOut = true,
                    UserId = user.Id,
                    Reason = LockoutReason.TooManyFailedLoginAttempts,
                };
                
                return Results.BadRequest(
                    @"Account is locked out due to too many failed login attempts. 
                            We sent letter with instruction to your email address", response);
            }
            else
            {
                var response = new LoginResponse()
                {
                    FailedLoginAttempts = user.FailedLoginAttempts,
                    IsLockedOut = false,
                    UserId = user.Id,
                };

                return Results.BadRequest("The password is not valid.", response);
            }
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
                IsLockedOut = lockoutState.IsActive,
                TwoFactorEnabled = true,
                UserId = user.Id
            });
        }

        var accessToken = await tokenManager.GenerateAsync(user, TokenType.Access, cancellationToken);
        var refreshToken = await tokenManager.GenerateAsync(user, TokenType.Refresh, cancellationToken);

        return Result.Success(new LoginResponse()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            UserId = user.Id,
            Message = "Successfully logged in.",
            TwoFactorEnabled = user.TwoFactorEnabled,
            IsLockedOut = lockoutState.IsActive,
        });
    }
}