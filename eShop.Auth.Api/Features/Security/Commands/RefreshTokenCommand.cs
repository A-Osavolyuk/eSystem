using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record RefreshTokenCommand(RefreshTokenRequest Request) : IRequest<Result>;

public class RefreshTokenCommandHandler(
    ITokenManager tokenManager,
    IUserManager userManager) : IRequestHandler<RefreshTokenCommand, Result>
{
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var userId = request.Request.UserId;
        var token = request.Request.Token;
        
        var user = await userManager.FindByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with id {userId}.");
        }
        
        var verificationResult = await tokenManager.VerifyAsync(user, token, cancellationToken);

        if (!verificationResult.Succeeded)
        {
            return verificationResult;
        }
        
        var accessToken = await tokenManager.GenerateAsync(user, TokenType.Access, cancellationToken);
        var refreshToken = await tokenManager.GenerateAsync(user, TokenType.Refresh, cancellationToken);
        
        var response = new RefreshTokenResponse()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

        return Result.Success(response);
    }
}