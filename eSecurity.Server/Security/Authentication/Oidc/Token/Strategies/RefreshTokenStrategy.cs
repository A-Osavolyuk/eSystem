using eSecurity.Core.Common.Responses;
using eSecurity.Core.Security.Cryptography.Protection.Constants;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.Oidc.Client;
using eSecurity.Server.Security.Cryptography.Keys;
using eSecurity.Server.Security.Cryptography.Tokens;
using eSecurity.Server.Security.Cryptography.Tokens.Jwt;
using eSecurity.Server.Security.Identity.Claims;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Security.Authentication.Oidc.Constants;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Server.Security.Authentication.Oidc.Token.Strategies;

public sealed class RefreshTokenPayload : TokenPayload
{
    public string? ClientSecret { get; set; }
    public string? RefreshToken { get; set; }
    public string? RedirectUri { get; set; }
    public string? Scope { get; set; }
}

public class RefreshTokenStrategy(
    IDataProtectionProvider protectionProvider,
    ITokenFactory tokenFactory,
    IClientManager clientManager,
    ITokenManager tokenManager,
    IUserManager userManager,
    IKeyFactory keyFactory,
    IClaimBuilderFactory claimBuilderFactory,
    IOptions<TokenOptions> options) : ITokenStrategy
{
    private readonly IDataProtectionProvider _protectionProvider = protectionProvider;
    private readonly ITokenFactory _tokenFactory = tokenFactory;
    private readonly IClientManager _clientManager = clientManager;
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly IUserManager _userManager = userManager;
    private readonly IKeyFactory _keyFactory = keyFactory;
    private readonly IClaimBuilderFactory _claimBuilderFactory = claimBuilderFactory;
    private readonly TokenOptions _options = options.Value;

    public async ValueTask<Result> ExecuteAsync(TokenPayload payload,
        CancellationToken cancellationToken = default)
    {
        if (payload is not RefreshTokenPayload refreshPayload)
            throw new NotSupportedException("Payload type must be 'RefreshTokenPayload'.");

        var client = await _clientManager.FindByIdAsync(refreshPayload.ClientId, cancellationToken);
        if (client is null)
            return Results.Unauthorized(new Error()
            {
                Code = Errors.OAuth.InvalidClient,
                Description = "Invalid client."
            });

        if (!client.HasGrantType(refreshPayload.GrantType))
            return Results.BadRequest(new Error()
            {
                Code = Errors.OAuth.UnsupportedGrantType,
                Description = $"'{refreshPayload.GrantType}' is not supported by client."
            });

        var protector = _protectionProvider.CreateProtector(ProtectionPurposes.RefreshToken);
        var unprotectedToken = protector.Unprotect(refreshPayload.RefreshToken!);

        var refreshToken = await _tokenManager.FindByTokenAsync(unprotectedToken, cancellationToken);
        if (refreshToken is null || !refreshToken.IsValid)
            return Results.NotFound(new Error()
            {
                Code = Errors.OAuth.InvalidGrant,
                Description = "Invalid refresh token."
            });

        if (refreshToken.Revoked)
        {
            //TODO: Implement revoked token reuse detection
        }

        if (client.Id != refreshToken.ClientId)
            return Results.BadRequest(new Error()
            {
                Code = Errors.OAuth.InvalidGrant,
                Description = "Invalid refresh token"
            });

        var requestedScopes = string.IsNullOrEmpty(refreshPayload.Scope)
            ? client.AllowedScopes.Select(x => x.Scope.Name).ToList()
            : refreshPayload.Scope!.Split(' ').ToList();
        
        if (!client.HasScopes(requestedScopes))
            return Results.BadRequest(new Error()
            {
                Code = Errors.OAuth.InvalidScope,
                Description = "Requested scopes exceed originally granted scopes."
            });
        
        if (requestedScopes.Contains(Scopes.OfflineAccess) && !client.AllowOfflineAccess)
            return Results.BadRequest(new Error()
            {
                Code = Errors.OAuth.InvalidScope,
                Description = "offline_access scope was not originally granted."
            });

        if (client is { Type: ClientType.Confidential, RequireClientSecret: true })
        {
            if (string.IsNullOrEmpty(refreshPayload.ClientSecret)
                || !client.Secret.Equals(refreshPayload.ClientSecret))
            {
                return Results.Unauthorized(new Error()
                {
                    Code = Errors.OAuth.InvalidClient,
                    Description = "Client authentication failed."
                });
            }
        }

        var device = refreshToken.Session.Device;
        var user = await _userManager.FindByIdAsync(device.UserId, cancellationToken);
        if (user is null)
            return Results.NotFound(new Error()
            {
                Code = Errors.OAuth.InvalidGrant,
                Description = "Invalid refresh token."
            });

        var accessTokenClaims = _claimBuilderFactory.CreateAccessBuilder()
            .WithIssuer(_options.Issuer)
            .WithAudience(client.Audience)
            .WithSubject(user.Id.ToString())
            .WithTokenId(Guid.CreateVersion7().ToString())
            .WithSessionId(refreshToken.Session.Id.ToString())
            .WithIssuedTime(DateTimeOffset.UtcNow)
            .WithExpirationTime(DateTimeOffset.UtcNow.Add(_options.AccessTokenLifetime))
            .WithScope(client.AllowedScopes.Select(x => x.Scope.Name))
            .Build();

        var response = new TokenResponse()
        {
            AccessToken = await _tokenFactory.CreateAsync(accessTokenClaims, cancellationToken),
            ExpiresIn = (int)_options.AccessTokenLifetime.TotalSeconds,
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
                Token = _keyFactory.Create(20),
                ExpireDate = DateTimeOffset.UtcNow.Add(client.RefreshTokenLifetime),
                CreateDate = DateTimeOffset.UtcNow
            };

            var createResult = await _tokenManager.CreateAsync(newRefreshToken, cancellationToken);
            if (!createResult.Succeeded) return createResult;

            var revokeResult = await _tokenManager.RevokeAsync(refreshToken, cancellationToken);
            if (!revokeResult.Succeeded) return revokeResult;

            var protectedToken = protector.Protect(newRefreshToken.Token);
            response.RefreshToken = protectedToken;
        }
        else
        {
            var protectedToken = protector.Protect(refreshToken.Token);
            response.RefreshToken = protectedToken;
        }

        if (requestedScopes.Contains(Scopes.OpenId))
        {
            var idClaims = _claimBuilderFactory.CreateIdBuilder()
                .WithOpenId(user, requestedScopes)
                .WithIssuer(_options.Issuer)
                .WithAudience(client.Audience)
                .WithSubject(user.Id.ToString())
                .WithTokenId(Guid.CreateVersion7().ToString())
                .WithSessionId(refreshToken.Session.Id.ToString())
                .WithIssuedTime(DateTimeOffset.UtcNow)
                .WithAuthenticationTime(DateTimeOffset.UtcNow)
                .WithExpirationTime(DateTimeOffset.UtcNow.Add(_options.IdTokenLifetime))
                .Build();

            response.IdToken = await _tokenFactory.CreateAsync(idClaims, cancellationToken);
        }

        return Results.Ok(response);
    }
}