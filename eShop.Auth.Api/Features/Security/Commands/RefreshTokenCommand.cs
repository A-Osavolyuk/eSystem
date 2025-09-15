using eShop.Domain.Common.Security.Constants;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record RefreshTokenCommand() : IRequest<Result>;

public class RefreshTokenCommandHandler(
    ITokenManager tokenManager,
    IUserManager userManager,
    TokenHandler tokenHandler) : IRequestHandler<RefreshTokenCommand, Result>
{
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly IUserManager userManager = userManager;
    private readonly TokenHandler tokenHandler = tokenHandler;

    public async Task<Result> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var token = tokenHandler.Get();
        if (string.IsNullOrEmpty(token)) return Results.BadRequest("Token is invalid");
        
        var claims = tokenHandler.Read(token);
        if(claims.Count == 0) return Results.BadRequest("Token is invalid");

        var userClaim = claims.FirstOrDefault(x => x.Type == AppClaimTypes.Subject);
        if (userClaim is null) return Results.BadRequest("Token is invalid");

        var userId = Guid.Parse(userClaim.Value);
        var user = await userManager.FindByIdAsync(userId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with id {userClaim}.");

        var accessToken = await tokenManager.GenerateAsync(user, cancellationToken);
        var response = new RefreshTokenResponse() { Token = accessToken };

        return Result.Success(response);
    }
}