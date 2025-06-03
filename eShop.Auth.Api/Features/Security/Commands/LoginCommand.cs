using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record LoginCommand(LoginRequest Request) : IRequest<Result>;

internal sealed class LoginCommandHandler(
    ITokenManager tokenManager,
    IUserManager userManager,
    ILockoutManager lockoutManager) : IRequestHandler<LoginCommand, Result>
{
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly IUserManager userManager = userManager;
    private readonly ILockoutManager lockoutManager = lockoutManager;

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

        var isValidPassword = await userManager.CheckPasswordAsync(user, request.Request.Password, cancellationToken);

        if (!isValidPassword)
        {
            return Results.BadRequest("The password is not valid.");
        }
        
        var lockoutState = await lockoutManager.FindAsync(user, cancellationToken);

        if (lockoutState.IsActive)
        {
            return Result.Success(new LoginResponse()
            {
                UserId = user.Id,
                Message = "Successfully logged in.",
                IsLockedOut = lockoutState.IsActive,
            });
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