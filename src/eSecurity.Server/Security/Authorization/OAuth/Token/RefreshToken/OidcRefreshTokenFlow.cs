using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Constants;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Server.Security.Cryptography.Hashing;
using eSecurity.Server.Security.Cryptography.Tokens;
using eSecurity.Server.Security.Identity.Claims;
using eSecurity.Server.Security.Identity.Claims.Factories;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;
using eSystem.Core.Security.Authorization.OAuth.Constants;
using eSystem.Core.Security.Authorization.OAuth.Token.RefreshToken;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.RefreshToken;

public sealed class OidcRefreshTokenFlow(
    ITokenFactoryProvider tokenFactoryProvider,
    IClientManager clientManager,
    ITokenManager tokenManager,
    IUserManager userManager,
    IHasherProvider hasherProvider,
    ISessionManager sessionManager,
    IClaimFactoryProvider claimFactoryProvider,
    IOptions<TokenConfigurations> options) : IRefreshTokenFlow
{
    private readonly ITokenFactoryProvider _tokenFactoryProvider = tokenFactoryProvider;
    private readonly IClientManager _clientManager = clientManager;
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly IUserManager _userManager = userManager;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly IClaimFactoryProvider _claimFactoryProvider = claimFactoryProvider;
    private readonly TokenConfigurations _tokenConfigurations = options.Value;
    private readonly IHasher _hasher = hasherProvider.GetHasher(HashAlgorithm.Sha512);

    public async ValueTask<Result> ExecuteAsync(OpaqueTokenEntity token, RefreshTokenFlowContext flowContext,
        CancellationToken cancellationToken = default)
    {
        if (!token.SessionId.HasValue)
        {
            return Results.NotFound(new Error
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Invalid refresh token."
            });
        }

        var session = await _sessionManager.FindByIdAsync(token.SessionId.Value, cancellationToken);
        if (session is null)
        {
            return Results.NotFound(new Error
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Invalid refresh token."
            });
        }

        if (token.Revoked)
        {
            var sessionResult = await _sessionManager.RemoveAsync(session, cancellationToken);
            if (!sessionResult.Succeeded) return sessionResult;

            return Results.NotFound(new Error
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Invalid refresh token."
            });
        }

        var client = await _clientManager.FindByIdAsync(flowContext.ClientId, cancellationToken);
        if (client is null)
        {
            return Results.Unauthorized(new Error
            {
                Code = ErrorTypes.OAuth.InvalidClient,
                Description = "Invalid client."
            });
        }

        if (!client.HasGrantType(flowContext.GrantType))
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.UnsupportedGrantType,
                Description = $"'{flowContext.GrantType}' is not supported by client."
            });
        }

        if (client.Id != token.ClientId)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Invalid refresh token"
            });
        }

        if (!client.HasScope(ScopeTypes.OfflineAccess))
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidScope,
                Description = "offline_access scope was not originally granted."
            });
        }

        if (!client.AllowOfflineAccess)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Refresh token grant is not allowed for this client."
            });
        }

        var user = await _userManager.FindByIdAsync(Guid.Parse(token.Subject), cancellationToken);
        if (user is null)
        {
            return Results.NotFound(new Error
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Invalid refresh token."
            });
        }

        var response = new RefreshTokenResponse
        {
            ExpiresIn = (int)_tokenConfigurations.DefaultAccessTokenLifetime.TotalSeconds,
            TokenType = ResponseTokenTypes.Bearer,
        };

        if (client.AccessTokenType == AccessTokenType.Jwt)
        {
            var claimsFactory = _claimFactoryProvider.GetClaimFactory<AccessTokenClaimsContext, UserEntity>();
            var claims = await claimsFactory.GetClaimsAsync(user, new AccessTokenClaimsContext
            {
                Aud = client.Audiences.Select(x => x.Audience),
                Scopes = client.AllowedScopes.Select(x => x.Scope.Value),
            }, cancellationToken);

            var tokenContext = new JwtTokenContext { Claims = claims, Type = JwtTokenTypes.AccessToken };
            var tokenFactory = _tokenFactoryProvider.GetFactory<JwtTokenContext, string>();

            response.AccessToken = await tokenFactory.CreateTokenAsync(tokenContext, cancellationToken);
        }
        else
        {
            var tokenContext = new OpaqueTokenContext
            {
                TokenLength = _tokenConfigurations.OpaqueTokenLength,
                TokenType = OpaqueTokenType.AccessToken,
                ClientId = client.Id,
                Audiences = client.Audiences.Select(x => x.Audience).ToList(),
                Scopes = client.AllowedScopes.Select(x => x.Scope.Value).ToList(),
                ExpiredAt = DateTimeOffset.UtcNow.Add(_tokenConfigurations.DefaultAccessTokenLifetime),
                Subject = user.Id.ToString(),
                Sid = session.Id
            };
            var tokenFactory = _tokenFactoryProvider.GetFactory<OpaqueTokenContext, string>();
            response.AccessToken = await tokenFactory.CreateTokenAsync(tokenContext, cancellationToken);
        }

        if (client.RefreshTokenRotationEnabled)
        {
            var lifetime = client.RefreshTokenLifetime ?? _tokenConfigurations.DefaultRefreshTokenLifetime;
            var tokenContext = new OpaqueTokenContext
            {
                TokenLength = _tokenConfigurations.OpaqueTokenLength,
                TokenType = OpaqueTokenType.RefreshToken,
                ClientId = client.Id,
                Audiences = client.Audiences.Select(x => x.Audience).ToList(),
                Scopes = client.AllowedScopes.Select(x => x.Scope.Value).ToList(),
                ExpiredAt = DateTimeOffset.UtcNow.Add(lifetime),
                Subject = user.Id.ToString(),
                Sid = session.Id
            };
            
            var refreshTokenFactory = _tokenFactoryProvider.GetFactory<OpaqueTokenContext, string>();
            response.RefreshToken = await refreshTokenFactory.CreateTokenAsync(tokenContext, cancellationToken);
        }
        else
        {
            response.RefreshToken = flowContext.RefreshToken;
        }

        var idClaimsFactory = _claimFactoryProvider.GetClaimFactory<IdTokenClaimsContext, UserEntity>();
        var idClaims = await idClaimsFactory.GetClaimsAsync(user, new IdTokenClaimsContext
        {
            Aud = client.Id.ToString(),
            Scopes = client.AllowedScopes.Select(x => x.Scope.Value),
            Sid = session.Id.ToString(),
            AuthTime = DateTimeOffset.UtcNow,
            AuthenticationMethods = session.AuthenticationMethods,
        }, cancellationToken);

        var idTokenContext = new JwtTokenContext { Claims = idClaims, Type = JwtTokenTypes.IdToken };
        var idTokenFactory = _tokenFactoryProvider.GetFactory<JwtTokenContext, string>();

        response.IdToken = await idTokenFactory.CreateTokenAsync(idTokenContext, cancellationToken);

        return Results.Ok(response);
    }
}