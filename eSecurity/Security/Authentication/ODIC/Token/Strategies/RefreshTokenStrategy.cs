using eSecurity.Data.Entities;
using eSecurity.Security.Authentication.JWT;
using eSecurity.Security.Authentication.JWT.Constants;
using eSecurity.Security.Identity.User;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Responses.Auth;
using eSystem.Core.Security.Authentication.JWT;
using eSystem.Core.Security.Authentication.JWT.Claims;
using eSystem.Core.Security.Authentication.ODIC.Client;
using eSystem.Core.Security.Authentication.ODIC.Constants;
using eSystem.Core.Security.Cryptography.Keys;
using eSystem.Core.Security.Cryptography.Protection;

namespace eSecurity.Security.Authentication.ODIC.Token.Strategies;

public class RefreshTokenStrategy(
    IProtectorFactory protectorFactory,
    ITokenFactory tokenFactory,
    ITokenManager tokenManager,
    IUserManager userManager,
    IKeyFactory keyFactory,
    IOptions<JwtOptions> options) : TokenStrategy
{
    private readonly IProtectorFactory protectorFactory = protectorFactory;
    private readonly ITokenFactory tokenFactory = tokenFactory;
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly IUserManager userManager = userManager;
    private readonly IKeyFactory keyFactory = keyFactory;
    private readonly JwtOptions options = options.Value;

    public override async ValueTask<Result> HandleAsync(TokenRequest request,
        CancellationToken cancellationToken = default)
    {
        var protector = protectorFactory.Create(ProtectionPurposes.RefreshToken);
        var unprotectedToken = protector.Unprotect(request.RefreshToken!);

        var refreshToken = await tokenManager.FindByTokenAsync(unprotectedToken, cancellationToken);
        if (refreshToken is null) return Results.NotFound("Refresh token not found.");
        if (!refreshToken.IsValid) return Results.BadRequest("Refresh token is invalid.");

        if (refreshToken.Revoked)
        {
            //TODO: Implement revoked token reuse detection
        }

        var client = refreshToken.Client;
        if (!client.ClientId.Equals(request.ClientId)) 
            return Results.BadRequest("Invalid client ID.");

        if (!client.AllowOfflineAccess || !client.HasScope(Scopes.OfflineAccess))
            return Results.BadRequest("Offline access is not allowed for client.");

        if (client is { Type: ClientType.Confidential, RequireClientSecret: true })
        {
            if (string.IsNullOrEmpty(request.ClientSecret))
                return Results.BadRequest("Client secret is required.");

            if (!client.ClientSecret.Equals(request.ClientSecret))
                return Results.BadRequest("Invalid client secret.");
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

        var accessTokenClaims = ClaimBuilder.Create()
            .WithIssuer(options.Issuer)
            .WithAudience(client.Name)
            .WithSubject(user.Id.ToString())
            .WithTokenId(Guid.CreateVersion7().ToString())
            .WithIssuedTime(DateTimeOffset.UtcNow)
            .WithExpirationTime(DateTimeOffset.UtcNow.AddDays(options.AccessTokenExpirationMinutes))
            .WithScope(client.AllowedScopes.Select(x => x.Scope.Name))
            .Build();

        var accessToken = tokenFactory.Create(accessTokenClaims);
        var protectedRefreshToken = protector.Protect(newRefreshToken.Token);
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