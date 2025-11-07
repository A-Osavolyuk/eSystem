using eSecurity.Common.Responses;
using eSecurity.Data.Entities;
using eSecurity.Features.Odic.Commands;
using eSecurity.Security.Cryptography.Tokens;
using eSecurity.Security.Cryptography.Tokens.Jwt;
using eSecurity.Security.Identity.Claims;
using eSecurity.Security.Identity.User;
using eSystem.Core.Security.Authentication.Jwt;
using eSystem.Core.Security.Authentication.Odic.Client;
using eSystem.Core.Security.Authentication.Odic.Constants;
using eSystem.Core.Security.Cryptography.Keys;
using eSystem.Core.Security.Cryptography.Protection;

namespace eSecurity.Security.Authentication.Odic.Token.Strategies;

public sealed class RefreshTokenPayload : TokenPayload
{
    public required string ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? RefreshToken { get; set; }
    public string? RedirectUri { get; set; }
}

public class RefreshTokenStrategy(
    IProtectorFactory protectorFactory,
    ITokenFactory tokenFactory,
    ITokenManager tokenManager,
    IUserManager userManager,
    IKeyFactory keyFactory,
    IClaimBuilderFactory claimBuilderFactory,
    IOptions<JwtOptions> options) : ITokenStrategy
{
    private readonly IProtectorFactory protectorFactory = protectorFactory;
    private readonly ITokenFactory tokenFactory = tokenFactory;
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly IUserManager userManager = userManager;
    private readonly IKeyFactory keyFactory = keyFactory;
    private readonly IClaimBuilderFactory claimBuilderFactory = claimBuilderFactory;
    private readonly JwtOptions options = options.Value;

    public async ValueTask<Result> ExecuteAsync(TokenPayload payload,
        CancellationToken cancellationToken = default)
    {
        if(payload is not RefreshTokenPayload refreshPayload)
            throw new NotSupportedException("Payload type must be 'RefreshTokenPayload'.");
        
        var protector = protectorFactory.Create(ProtectionPurposes.RefreshToken);
        var unprotectedToken = protector.Unprotect(refreshPayload.RefreshToken!);

        var refreshToken = await tokenManager.FindByTokenAsync(unprotectedToken, cancellationToken);
        if (refreshToken is null) return Results.NotFound("Refresh token not found.");
        if (!refreshToken.IsValid) return Results.BadRequest("Refresh token is invalid.");

        if (refreshToken.Revoked)
        {
            //TODO: Implement revoked token reuse detection
        }

        var client = refreshToken.Client;
        if (!client.ClientId.Equals(refreshPayload.ClientId)) 
            return Results.BadRequest("Invalid client ID.");

        if (!client.AllowOfflineAccess || !client.HasScope(Scopes.OfflineAccess))
            return Results.BadRequest("Offline access is not allowed for client.");

        if (client is { Type: ClientType.Confidential, RequireClientSecret: true })
        {
            if (string.IsNullOrEmpty(refreshPayload.ClientSecret))
                return Results.BadRequest("Client secret is required.");

            if (!client.ClientSecret.Equals(refreshPayload.ClientSecret))
                return Results.BadRequest("Invalid client secret.");
        }

        var device = refreshToken.Session.Device;
        var user = await userManager.FindByIdAsync(device.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

        var accessTokenClaims = claimBuilderFactory.CreateAccessBuilder()
            .WithIssuer(options.Issuer)
            .WithAudience(client.Name)
            .WithSubject(user.Id.ToString())
            .WithTokenId(Guid.CreateVersion7().ToString())
            .WithSessionId(refreshToken.Session.Id.ToString())
            .WithIssuedTime(DateTimeOffset.UtcNow)
            .WithExpirationTime(DateTimeOffset.UtcNow.AddMinutes(options.AccessTokenExpirationMinutes))
            .WithScope(client.AllowedScopes.Select(x => x.Scope.Name))
            .Build();
        
        var response = new TokenResponse()
        {
            AccessToken = await tokenFactory.CreateAsync(accessTokenClaims),
            ExpiresIn = options.AccessTokenExpirationMinutes * 60,
            TokenType = TokenTypes.Bearer,
        };

        if (client.RefreshTokenRotationEnabled)
        {
            var session = refreshToken.Session;
            var newRefreshToken = new RefreshTokenEntity()
            {
                Id = Guid.CreateVersion7(),
                ClientId = client.Id,
                SessionId = session.Id,
                Token = keyFactory.Create(20),
                ExpireDate = DateTimeOffset.UtcNow.Add(client.RefreshTokenLifetime),
                CreateDate = DateTimeOffset.UtcNow
            };
            
            var createResult = await tokenManager.CreateAsync(newRefreshToken, cancellationToken);
            if (!createResult.Succeeded) return createResult;

            var revokeResult = await tokenManager.RevokeAsync(refreshToken, cancellationToken);
            if (!revokeResult.Succeeded) return revokeResult;
            
            var protectedToken = protector.Protect(newRefreshToken.Token);
            response.RefreshToken = protectedToken;
        }
        else
        {
            var protectedToken = protector.Protect(refreshToken.Token);
            response.RefreshToken = protectedToken;
        }
        
        if (client.HasScope(Scopes.OpenId))
        {
            var idClaims = claimBuilderFactory.CreateIdBuilder()
                .WithOpenId(user, client)
                .WithIssuer(options.Issuer)
                .WithAudience(client.Name)
                .WithSubject(user.Id.ToString())
                .WithTokenId(Guid.CreateVersion7().ToString())
                .WithSessionId(refreshToken.Session.Id.ToString())
                .WithIssuedTime(DateTimeOffset.UtcNow)
                .WithAuthenticationTime(DateTimeOffset.UtcNow)
                .WithExpirationTime(DateTimeOffset.UtcNow.AddMinutes(options.AccessTokenExpirationMinutes))
                .Build();
            
            response.IdToken = await tokenFactory.CreateAsync(idClaims);
        }

        return Result.Success(response);
    }
}