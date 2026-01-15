using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.Oidc.Client;
using eSecurity.Server.Security.Authentication.Oidc.Code;
using eSecurity.Server.Security.Authentication.Oidc.Pkce;
using eSecurity.Server.Security.Authentication.Oidc.Session;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Cryptography.Hashing;
using eSecurity.Server.Security.Cryptography.Tokens;
using eSecurity.Server.Security.Identity.Claims;
using eSecurity.Server.Security.Identity.Claims.Factories;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authentication.Oidc.Constants;
using eSystem.Core.Security.Authentication.Oidc.Token;
using eSystem.Core.Security.Cryptography.Protection.Constants;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Server.Security.Authentication.Oidc.Token.Strategies;

public sealed class AuthorizationCodeTokenPayload : TokenPayload
{
    public string? RedirectUri { get; set; }
    public string? Code { get; set; }
    public string? CodeVerifier { get; set; }
}

public class AuthorizationCodeStrategy(
    IUserManager userManager,
    IPkceHandler pkceHandler,
    ITokenManager tokenManager,
    IDeviceManager deviceManager,
    IClientManager clientManager,
    IHasherProvider hasherProvider,
    ISessionManager sessionManager,
    IDataProtectionProvider protectionProvider,
    IClaimFactoryProvider claimFactoryProvider,
    ITokenFactoryProvider tokenFactoryProvider,
    IAuthorizationCodeManager authorizationCodeManager,
    IOptions<TokenOptions> options) : ITokenStrategy
{
    private readonly IUserManager _userManager = userManager;
    private readonly IClientManager _clientManager = clientManager;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly IAuthorizationCodeManager _authorizationCodeManager = authorizationCodeManager;
    private readonly ITokenFactoryProvider _tokenFactoryProvider = tokenFactoryProvider;
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly IPkceHandler _pkceHandler = pkceHandler;
    private readonly IHasherProvider _hasherProvider = hasherProvider;
    private readonly IDataProtectionProvider _protectionProvider = protectionProvider;
    private readonly IClaimFactoryProvider _claimFactoryProvider = claimFactoryProvider;
    private readonly TokenOptions _options = options.Value;

    public async ValueTask<Result> ExecuteAsync(TokenPayload payload,
        CancellationToken cancellationToken = default)
    {
        if (payload is not AuthorizationCodeTokenPayload authorizationPayload)
            throw new NotSupportedException("Payload type must be 'AuthorizationCodeTokenPayload'");

        if (string.IsNullOrEmpty(authorizationPayload.RedirectUri))
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "redirect_uri is required"
            });

        var client = await _clientManager.FindByIdAsync(authorizationPayload.ClientId, cancellationToken);
        if (client is null)
            return Results.Unauthorized(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidClient,
                Description = "Client was not found."
            });

        if (!client.HasGrantType(authorizationPayload.GrantType))
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.UnsupportedGrantType,
                Description = $"'{authorizationPayload.GrantType}' grant is not supported by client."
            });

        var code = authorizationPayload.Code!;
        var redirectUri = authorizationPayload.RedirectUri;
        var authorizationCode = await _authorizationCodeManager.FindByCodeAsync(code, cancellationToken);

        if (authorizationCode is null || authorizationCode.Used ||
            authorizationCode.ExpireDate < DateTimeOffset.UtcNow ||
            string.IsNullOrEmpty(redirectUri) ||
            !authorizationCode.RedirectUri.Equals(redirectUri) ||
            client.Id != authorizationCode.ClientId ||
            !client.HasUri(redirectUri, UriType.Redirect))
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Invalid authorization code."
            });
        }

        if (client is { ClientType: ClientType.Public, RequirePkce: true })
        {
            if (string.IsNullOrWhiteSpace(authorizationCode.CodeChallenge)
                || string.IsNullOrWhiteSpace(authorizationCode.CodeChallengeMethod)
                || string.IsNullOrWhiteSpace(authorizationPayload.CodeVerifier))
            {
                return Results.BadRequest(new Error()
                {
                    Code = ErrorTypes.OAuth.InvalidGrant,
                    Description = "Invalid authorization code."
                });
            }

            var isValidPkce = _pkceHandler.Verify(
                authorizationCode.CodeChallenge,
                authorizationCode.CodeChallengeMethod,
                authorizationPayload.CodeVerifier
            );

            if (!isValidPkce)
                return Results.BadRequest(new Error()
                {
                    Code = ErrorTypes.OAuth.InvalidGrant,
                    Description = "Invalid authorization code."
                });
        }

        var device = await _deviceManager.FindByIdAsync(authorizationCode.DeviceId, cancellationToken);
        if (device is null)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Invalid authorization code."
            });
        }
        
        var session = await _sessionManager.FindAsync(device, cancellationToken);
        var user = await _userManager.FindByIdAsync(device.UserId, cancellationToken);
        if (session is null || session.ExpireDate < DateTimeOffset.UtcNow || user is null)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Invalid authorization code."
            });
        }

        var codeResult = await _authorizationCodeManager.UseAsync(authorizationCode, cancellationToken);
        if (!codeResult.Succeeded) return codeResult;

        var accessClaimFactory = _claimFactoryProvider.GetClaimFactory<AccessClaimsContext>();
        var accessClaims = await accessClaimFactory.GetClaimsAsync(user, new AccessClaimsContext()
        {
            Aud = client.Audience,
            Scopes = client.AllowedScopes.Select(x => x.Scope.Name),
            Sid = session.Id.ToString(),
            Nonce = authorizationCode.Nonce
        }, cancellationToken);

        var accessTokenContext = new JwtTokenContext { Claims = accessClaims };
        var jwtTokenFactory = _tokenFactoryProvider.GetFactory<JwtTokenContext, string>();
        var response = new TokenResponse()
        {
            AccessToken = await jwtTokenFactory.CreateTokenAsync(accessTokenContext, cancellationToken),
            ExpiresIn = (int)_options.AccessTokenLifetime.TotalSeconds,
            TokenType = TokenTypes.Bearer,
        };

        if (client.AllowOfflineAccess && client.HasScope(Scopes.OfflineAccess))
        {
            var opaqueTokenFactory = _tokenFactoryProvider.GetFactory<OpaqueTokenContext, string>();
            var hasher = _hasherProvider.GetHasher(HashAlgorithm.Sha512);
            var refreshTokenContext = new OpaqueTokenContext() { Length = _options.RefreshTokenLength };
            var rawToken = await opaqueTokenFactory.CreateTokenAsync(refreshTokenContext, cancellationToken);
            var hash = hasher.Hash(rawToken);
            var refreshToken = new OpaqueTokenEntity()
            {
                Id = Guid.CreateVersion7(),
                ClientId = client.Id,
                SessionId = session.Id,
                TokenHash = hash,
                TokenType = OpaqueTokenType.RefreshToken,
                ExpiredDate = DateTimeOffset.UtcNow.Add(client.RefreshTokenLifetime)
            };

            var scopes = client.AllowedScopes.Select(x => x.Scope);
            var tokenResult = await _tokenManager.CreateAsync(refreshToken, scopes, cancellationToken);
            if (!tokenResult.Succeeded) return tokenResult;

            var protector = _protectionProvider.CreateProtector(ProtectionPurposes.RefreshToken);
            var protectedRefreshToken = protector.Protect(rawToken);
            response.RefreshToken = protectedRefreshToken;
        }

        if (client.HasScope(Scopes.OpenId))
        {
            var idClaimFactory = _claimFactoryProvider.GetClaimFactory<IdClaimsContext>();
            var idClaims = await idClaimFactory.GetClaimsAsync(user, new IdClaimsContext()
            {
                Aud = client.Audience,
                Scopes = client.AllowedScopes.Select(x => x.Scope.Name),
                Sid = session.Id.ToString(),
                AuthTime = DateTimeOffset.UtcNow,
            }, cancellationToken);

            var idTokenContext = new JwtTokenContext { Claims = idClaims };
            response.IdToken = await jwtTokenFactory.CreateTokenAsync(idTokenContext, cancellationToken);
        }

        return Results.Ok(response);
    }
}