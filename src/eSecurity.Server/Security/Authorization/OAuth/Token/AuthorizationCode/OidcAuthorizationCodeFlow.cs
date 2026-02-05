using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Constants;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Server.Security.Cryptography.Hashing;
using eSecurity.Server.Security.Cryptography.Pkce;
using eSecurity.Server.Security.Cryptography.Tokens;
using eSecurity.Server.Security.Identity.Claims;
using eSecurity.Server.Security.Identity.Claims.Factories;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authentication.OpenIdConnect.Client;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;
using eSystem.Core.Security.Authorization.OAuth.Constants;
using eSystem.Core.Security.Authorization.OAuth.Token;
using eSystem.Core.Security.Authorization.OAuth.Token.AuthorizationCode;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.AuthorizationCode;

public class OidcAuthorizationCodeFlow(
    IUserManager userManager,
    IPkceHandler pkceHandler,
    ITokenManager tokenManager,
    IClientManager clientManager,
    IHasherProvider hasherProvider,
    ISessionManager sessionManager,
    IClaimFactoryProvider claimFactoryProvider,
    ITokenFactoryProvider tokenFactoryProvider,
    IAuthorizationCodeManager authorizationCodeManager,
    IOptions<TokenOptions> options) : IAuthorizationCodeFlow
{
    private readonly IClientManager _clientManager = clientManager;
    private readonly IHasherProvider _hasherProvider = hasherProvider;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly IClaimFactoryProvider _claimFactoryProvider = claimFactoryProvider;
    private readonly ITokenFactoryProvider _tokenFactoryProvider = tokenFactoryProvider;
    private readonly IUserManager _userManager = userManager;
    private readonly IPkceHandler _pkceHandler = pkceHandler;
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly IAuthorizationCodeManager _authorizationCodeManager = authorizationCodeManager;
    private readonly TokenOptions _options = options.Value;

    public async ValueTask<Result> ExecuteAsync(AuthorizationCodeEntity code,
        AuthorizationCodeFlowContext context, CancellationToken cancellationToken = default)
    {
        var client = await _clientManager.FindByIdAsync(context.ClientId, cancellationToken);
        if (client is null)
            return Results.Unauthorized(new Error
            {
                Code = ErrorTypes.OAuth.InvalidClient,
                Description = "Client was not found."
            });

        if (client.Id != code.ClientId || !client.HasUri(context.RedirectUri!, UriType.Redirect))
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Invalid authorization code."
            });
        }

        if (!client.HasGrantType(context.GrantType))
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.UnsupportedGrantType,
                Description = $"'{context.GrantType}' grant is not supported by client."
            });
        }

        var user = await _userManager.FindByIdAsync(code.UserId, cancellationToken);
        if (user is null)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Invalid authorization code."
            });
        }

        if (client is { ClientType: ClientType.Public, RequirePkce: true })
        {
            if (string.IsNullOrWhiteSpace(code.CodeChallenge)
                || string.IsNullOrWhiteSpace(code.CodeChallengeMethod)
                || string.IsNullOrWhiteSpace(context.CodeVerifier))
            {
                return Results.BadRequest(new Error
                {
                    Code = ErrorTypes.OAuth.InvalidGrant,
                    Description = "Invalid authorization code."
                });
            }

            var isValidPkce = _pkceHandler.Verify(code.CodeChallenge, code.CodeChallengeMethod, context.CodeVerifier);
            if (!isValidPkce)
            {
                return Results.BadRequest(new Error
                {
                    Code = ErrorTypes.OAuth.InvalidGrant,
                    Description = "Invalid authorization code."
                });
            }
        }

        var codeResult = await _authorizationCodeManager.UseAsync(code, cancellationToken);
        if (!codeResult.Succeeded) return codeResult;

        var response = new AuthorizationCodeResponse
        {
            ExpiresIn = (int)_options.AccessTokenLifetime.TotalSeconds,
            TokenType = ResponseTokenTypes.Bearer,
        };

        if (client.AccessTokenType == AccessTokenType.Jwt)
        {
            var claimsFactory = _claimFactoryProvider.GetClaimFactory<AccessTokenClaimsContext, UserEntity>();
            var claims = await claimsFactory.GetClaimsAsync(user, new AccessTokenClaimsContext
            {
                Aud = client.Audiences.Select(x => x.Audience),
                Scopes = client.AllowedScopes.Select(x => x.Scope.Value),
                Nonce = code.Nonce
            }, cancellationToken);

            var tokenContext = new JwtTokenContext { Claims = claims, Type = JwtTokenTypes.AccessToken };
            var tokenFactory = _tokenFactoryProvider.GetFactory<JwtTokenContext, string>();

            response.AccessToken = await tokenFactory.CreateTokenAsync(tokenContext, cancellationToken);
        }
        else
        {
            var tokenContext = new OpaqueTokenContext { Length = _options.RefreshTokenLength };
            var tokenFactory = _tokenFactoryProvider.GetFactory<OpaqueTokenContext, string>();
            var rawToken = await tokenFactory.CreateTokenAsync(tokenContext, cancellationToken);
            var hasher = _hasherProvider.GetHasher(HashAlgorithm.Sha512);
            var newRefreshToken = new OpaqueTokenEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = client.Id,
                Subject = user.Id.ToString(),
                TokenHash = hasher.Hash(rawToken),
                TokenType = OpaqueTokenType.AccessToken,
                ExpiredDate = DateTimeOffset.UtcNow.Add(_options.AccessTokenLifetime)
            };
            
            var createResult = await _tokenManager.CreateAsync(newRefreshToken, client.AllowedScopes, cancellationToken);
            if (!createResult.Succeeded) return createResult;

            response.AccessToken = rawToken;
        }

        var session = await _sessionManager.FindAsync(user, cancellationToken);
        if (session is null || session.ExpireDate < DateTimeOffset.UtcNow)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Invalid authorization code."
            });
        }

        var clientResult = await _clientManager.RelateAsync(client, session, cancellationToken);
        if (!clientResult.Succeeded) return clientResult;

        if (client.AllowOfflineAccess && client.HasScope(ScopeTypes.OfflineAccess))
        {
            var refreshTokenContext = new OpaqueTokenContext { Length = _options.RefreshTokenLength };
            var refreshTokenFactory = _tokenFactoryProvider.GetFactory<OpaqueTokenContext, string>();
            var rawToken = await refreshTokenFactory.CreateTokenAsync(refreshTokenContext, cancellationToken);
            var hasher = _hasherProvider.GetHasher(HashAlgorithm.Sha512);
            var refreshToken = new OpaqueTokenEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = client.Id,
                SessionId = session.Id,
                Subject = user.Id.ToString(),
                TokenHash = hasher.Hash(rawToken),
                TokenType = OpaqueTokenType.RefreshToken,
                ExpiredDate = DateTimeOffset.UtcNow.Add(client.RefreshTokenLifetime)
            };
            
            var tokenResult = await _tokenManager.CreateAsync(refreshToken, client.AllowedScopes, cancellationToken);
            if (!tokenResult.Succeeded) return tokenResult;

            response.RefreshToken = rawToken;
        }

        var idClaimsFactory = _claimFactoryProvider.GetClaimFactory<IdTokenClaimsContext, UserEntity>();
        var idClaims = await idClaimsFactory.GetClaimsAsync(user, new IdTokenClaimsContext
        {
            Aud = client.Id.ToString(),
            Nonce = code.Nonce,
            Scopes = client.AllowedScopes.Select(x => x.Scope.Value),
            Sid = session.Id.ToString(),
            AuthenticationMethods = session.AuthenticationMethods,
            AuthTime = DateTimeOffset.UtcNow,
        }, cancellationToken);

        var idTokenContext = new JwtTokenContext { Claims = idClaims, Type = JwtTokenTypes.IdToken };
        var idTokenFactory = _tokenFactoryProvider.GetFactory<JwtTokenContext, string>();

        response.IdToken = await idTokenFactory.CreateTokenAsync(idTokenContext, cancellationToken);
        return Results.Ok(response);
    }
}