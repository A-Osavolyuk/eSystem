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
        
        var user = await userManager.FindByIdAsync(userId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with id {userId}.");
        
        var accessToken = await tokenManager.GenerateAsync(user, cancellationToken);
        var response = new RefreshTokenResponse() { Token = accessToken };

        return Result.Success(response);
    }
}