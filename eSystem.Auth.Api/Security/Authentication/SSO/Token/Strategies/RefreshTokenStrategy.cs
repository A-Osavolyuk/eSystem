using System.Security.Claims;
using eSystem.Auth.Api.Data.Entities;
using eSystem.Auth.Api.Security.Authentication.JWT;
using eSystem.Auth.Api.Security.Identity.User;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Responses.Auth;
using eSystem.Core.Security.Authentication.JWT;
using eSystem.Core.Security.Cryptography.Protection;
using eSystem.Core.Security.Cryptography.Tokens;
using eSystem.Core.Security.Authentication.SSO.Client;
using eSystem.Core.Security.Cryptography.Keys;
using eSystem.Core.Security.Identity.Claims;

namespace eSystem.Auth.Api.Security.Authentication.SSO.Token.Strategies;

public class RefreshTokenStrategy(
    IProtectorFactory protectorFactory,
    ITokenManager tokenManager,
    IUserManager userManager,
    IKeyFactory keyFactory,
    ITokenFactory tokenFactory,
    IOptions<JwtOptions> options) : TokenStrategy
{
    private readonly IProtectorFactory protectorFactory = protectorFactory;
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly IUserManager userManager = userManager;
    private readonly IKeyFactory keyFactory = keyFactory;
    private readonly ITokenFactory tokenFactory = tokenFactory;
    private readonly JwtOptions options = options.Value;

    public override async ValueTask<Result> HandleAsync(TokenRequest request,
        CancellationToken cancellationToken = default)
    {
        var protector = protectorFactory.Create(ProtectionPurposes.RefreshToken);
        var unprotectedToken = protector.Unprotect(request.RefreshToken!);

        var refreshToken = await tokenManager.FindByTokenAsync(unprotectedToken, cancellationToken);
        if (refreshToken is null) return Results.NotFound("Refresh token not found.");
        if (!refreshToken.IsValid) return Results.BadRequest("Refresh token is invalid.");

        var client = refreshToken.Client;
        if (!client.ClientId.Equals(request.ClientId)) return Results.BadRequest("Invalid client ID.");

        if (client is { Type: ClientType.Confidential, RequireClientSecret: true })
        {
            if (string.IsNullOrEmpty(request.ClientSecret))
                return Results.BadRequest("Client secret is required.");

            if (!client.ClientSecret.Equals(request.ClientSecret))
                return Results.BadRequest("Invalid client secret.");
        }

        if (refreshToken.Revoked)
        {
            //TODO: Implement revoked token reuse detection
        }
        
        var device = refreshToken.Session.Device;
        var user = await userManager.FindByIdAsync(device.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

        var session = refreshToken.Session;
        var newRefreshToken = new RefreshTokenEntity()
        {
            Id = Guid.CreateVersion7(),
            ClientId = client.Id,
            SessionId = session.Id,
            Token = keyFactory.Create(20),
            ExpireDate = DateTimeOffset.UtcNow.AddDays(options.RefreshTokenExpirationDays),
            CreateDate = DateTimeOffset.UtcNow
        };

        var rotateResult = await tokenManager.RotateAsync(refreshToken, newRefreshToken, cancellationToken);
        if (!rotateResult.Succeeded) return rotateResult;

        var protectedRefreshToken = protector.Protect(newRefreshToken.Token);

        var claimBuilder = ClaimBuilder.Create()
            .WithTokenId(Guid.CreateVersion7().ToString())
            .WithSubject(user.Id.ToString())
            .WithAudience(client.Name)
            .WithScope(client.AllowedScopes.Select(x => x.Scope.Name))
            .WithExpirationTime(DateTimeOffset.UtcNow.AddMinutes(options.AccessTokenExpirationMinutes))
            .WithIssuedTime(DateTimeOffset.UtcNow);

        var accessToken = tokenFactory.Create(claimBuilder.Build());
        var response = new TokenResponse()
        {
            AccessToken = accessToken,
            ExpiresIn = options.AccessTokenExpirationMinutes * 60,
            TokenType = TokenTypes.Bearer,
            RefreshToken = protectedRefreshToken,
        };

        return Result.Success(response);
    }
}