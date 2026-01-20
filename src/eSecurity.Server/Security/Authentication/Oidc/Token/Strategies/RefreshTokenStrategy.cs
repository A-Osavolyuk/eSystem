using eSecurity.Core.Security.Cryptography.Protection.Constants;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.Oidc.Client;
using eSecurity.Server.Security.Authentication.Oidc.Session;
using eSecurity.Server.Security.Cryptography.Hashing;
using eSecurity.Server.Security.Cryptography.Tokens;
using eSecurity.Server.Security.Identity.Claims;
using eSecurity.Server.Security.Identity.Claims.Factories;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authentication.Oidc.Constants;
using eSystem.Core.Security.Authentication.Oidc.Token;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Server.Security.Authentication.Oidc.Token.Strategies;

public sealed class RefreshTokenPayload : TokenPayload
{
    public string? RefreshToken { get; set; }
    public string? RedirectUri { get; set; }
    public string? Scope { get; set; }
}

public class RefreshTokenStrategy(
    IDataProtectionProvider protectionProvider,
    ITokenFactoryProvider tokenFactoryProvider,
    IClientManager clientManager,
    ITokenManager tokenManager,
    IUserManager userManager,
    IHasherProvider hasherProvider,
    ISessionManager sessionManager,
    IClaimFactoryProvider claimFactoryProvider,
    IOptions<TokenOptions> options) : ITokenStrategy
{
    private readonly IDataProtectionProvider _protectionProvider = protectionProvider;
    private readonly ITokenFactoryProvider _tokenFactoryProvider = tokenFactoryProvider;
    private readonly IClientManager _clientManager = clientManager;
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly IUserManager _userManager = userManager;
    private readonly IHasherProvider _hasherProvider = hasherProvider;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly IClaimFactoryProvider _claimFactoryProvider = claimFactoryProvider;
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
                Code = ErrorTypes.OAuth.InvalidClient,
                Description = "Invalid client."
            });

        if (!client.HasGrantType(refreshPayload.GrantType))
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.UnsupportedGrantType,
                Description = $"'{refreshPayload.GrantType}' is not supported by client."
            });

        var protector = _protectionProvider.CreateProtector(ProtectionPurposes.RefreshToken);
        var hasher = _hasherProvider.GetHasher(HashAlgorithm.Sha512);
        var unprotectedToken = protector.Unprotect(refreshPayload.RefreshToken!);
        var incomingHash = hasher.Hash(unprotectedToken);
        var refreshToken = await _tokenManager.FindByHashAsync(incomingHash, cancellationToken);
        if (refreshToken is null || !refreshToken.IsValid)
            return Results.NotFound(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Invalid refresh token."
            });

        if (refreshToken.Revoked)
        {
            var sessionResult = await _sessionManager.RemoveAsync(refreshToken.Session, cancellationToken);
            if (!sessionResult.Succeeded)
            {
                return Results.InternalServerError(new Error()
                {
                    Code = ErrorTypes.OAuth.ServerError,
                    Description = "Server error"
                });
            }
        }

        if (client.Id != refreshToken.ClientId)
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Invalid refresh token"
            });

        var requestedScopes = string.IsNullOrEmpty(refreshPayload.Scope)
            ? client.AllowedScopes.Select(x => x.Scope.Name).ToList()
            : refreshPayload.Scope!.Split(' ').ToList();

        if (!client.HasScopes(requestedScopes))
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidScope,
                Description = "Requested scopes exceed originally granted scopes."
            });

        if (requestedScopes.Contains(Scopes.OfflineAccess) && !client.AllowOfflineAccess)
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidScope,
                Description = "offline_access scope was not originally granted."
            });

        var device = refreshToken.Session.Device;
        var user = await _userManager.FindByIdAsync(device.UserId, cancellationToken);
        if (user is null)
            return Results.NotFound(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Invalid refresh token."
            });

        var accessClaimFactory = _claimFactoryProvider.GetClaimFactory<AccessClaimsContext>();
        var accessTokenClaims = await accessClaimFactory.GetClaimsAsync(user, new AccessClaimsContext()
        {
            Aud = client.Audience,
            Scopes = client.AllowedScopes.Select(x => x.Scope.Name),
            Sid = refreshToken.Session.Id.ToString()
        }, cancellationToken);

        var accessTokenContext = new JwtTokenContext { Claims = accessTokenClaims };
        var jwtTokenFactory = _tokenFactoryProvider.GetFactory<JwtTokenContext, string>();
        var response = new TokenResponse()
        {
            AccessToken = await jwtTokenFactory.CreateTokenAsync(accessTokenContext, cancellationToken),
            ExpiresIn = (int)_options.AccessTokenLifetime.TotalSeconds,
            TokenType = TokenTypes.Bearer,
        };

        if (client.RefreshTokenRotationEnabled)
        {
            var opaqueTokenFactory = _tokenFactoryProvider.GetFactory<OpaqueTokenContext, string>();
            var refreshTokenContext = new OpaqueTokenContext() { Length = _options.RefreshTokenLength };
            var rawToken = await opaqueTokenFactory.CreateTokenAsync(refreshTokenContext, cancellationToken);
            var newRefreshToken = new OpaqueTokenEntity()
            {
                Id = Guid.CreateVersion7(),
                ClientId = client.Id,
                SessionId = refreshToken.Session.Id,
                TokenHash = hasher.Hash(rawToken),
                TokenType = OpaqueTokenType.RefreshToken,
                ExpiredDate = DateTimeOffset.UtcNow.Add(client.RefreshTokenLifetime)
            };

            var scopes = client.AllowedScopes.Select(x => x.Scope);
            var createResult = await _tokenManager.CreateAsync(newRefreshToken, scopes, cancellationToken);
            if (!createResult.Succeeded) return createResult;

            var revokeResult = await _tokenManager.RevokeAsync(refreshToken, cancellationToken);
            if (!revokeResult.Succeeded) return revokeResult;

            var protectedToken = protector.Protect(rawToken);
            response.RefreshToken = protectedToken;
        }
        else
        {
            var protectedToken = protector.Protect(refreshToken.TokenHash);
            response.RefreshToken = protectedToken;
        }

        if (requestedScopes.Contains(Scopes.OpenId))
        {
            var idClaimFactory = _claimFactoryProvider.GetClaimFactory<IdClaimsContext>();
            var idClaims = await idClaimFactory.GetClaimsAsync(user, new IdClaimsContext()
            {
                Aud = client.Id.ToString(),
                Scopes = client.AllowedScopes.Select(x => x.Scope.Name),
                Sid = refreshToken.Session.Id.ToString(),
                AuthTime = DateTimeOffset.UtcNow
            }, cancellationToken);

            var idTokenContext = new JwtTokenContext { Claims = idClaims };
            response.IdToken = await jwtTokenFactory.CreateTokenAsync(idTokenContext, cancellationToken);
        }

        return Results.Ok(response);
    }
}