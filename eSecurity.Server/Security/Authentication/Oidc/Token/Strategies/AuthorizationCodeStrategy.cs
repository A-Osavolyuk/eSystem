using eSecurity.Core.Common.Responses;
using eSecurity.Core.Security.Cryptography.Protection.Constants;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.Oidc.Client;
using eSecurity.Server.Security.Authentication.Oidc.Code;
using eSecurity.Server.Security.Authentication.Oidc.Pkce;
using eSecurity.Server.Security.Authentication.Oidc.Session;
using eSecurity.Server.Security.Cryptography.Hashing;
using eSecurity.Server.Security.Cryptography.Tokens;
using eSecurity.Server.Security.Identity.Claims;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Security.Authentication.Oidc;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Server.Security.Authentication.Oidc.Token.Strategies;

public sealed class AuthorizationCodeTokenPayload : TokenPayload
{
    public string? RedirectUri { get; set; }
    public string? Code { get; set; }
    public string? ClientSecret { get; set; }
    public string? CodeVerifier { get; set; }
}

public class AuthorizationCodeStrategy(
    IUserManager userManager,
    IClientManager clientManager,
    ISessionManager sessionManager,
    IAuthorizationCodeManager authorizationCodeManager,
    ITokenFactoryProvider tokenFactoryProvider,
    ITokenManager tokenManager,
    IPkceHandler pkceHandler,
    IHasherFactory hasherFactory,
    IDataProtectionProvider protectionProvider,
    IClaimBuilderFactory claimBuilderFactory,
    IOptions<TokenOptions> options) : ITokenStrategy
{
    private readonly IUserManager _userManager = userManager;
    private readonly IClientManager _clientManager = clientManager;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly IAuthorizationCodeManager _authorizationCodeManager = authorizationCodeManager;
    private readonly ITokenFactoryProvider _tokenFactoryProvider = tokenFactoryProvider;
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly IPkceHandler _pkceHandler = pkceHandler;
    private readonly IHasherFactory _hasherFactory = hasherFactory;
    private readonly IDataProtectionProvider _protectionProvider = protectionProvider;
    private readonly IClaimBuilderFactory _claimBuilderFactory = claimBuilderFactory;
    private readonly TokenOptions _options = options.Value;

    public async ValueTask<Result> ExecuteAsync(TokenPayload payload,
        CancellationToken cancellationToken = default)
    {
        if (payload is not AuthorizationCodeTokenPayload authorizationPayload)
            throw new NotSupportedException("Payload type must be 'AuthorizationCodeTokenPayload'");

        if (string.IsNullOrEmpty(authorizationPayload.RedirectUri))
            return Results.BadRequest(new Error()
            {
                Code = Errors.OAuth.InvalidRequest,
                Description = "redirect_uri is required"
            });

        var client = await _clientManager.FindByIdAsync(authorizationPayload.ClientId, cancellationToken);
        if (client is null)
            return Results.Unauthorized(new Error()
            {
                Code = Errors.OAuth.InvalidClient,
                Description = "Client was not found."
            });

        if (!client.HasGrantType(authorizationPayload.GrantType))
            return Results.BadRequest(new Error()
            {
                Code = Errors.OAuth.UnsupportedGrantType,
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
            !client.HasRedirectUri(redirectUri))
        {
            return Results.BadRequest(new Error()
            {
                Code = Errors.OAuth.InvalidGrant,
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
                    Code = Errors.OAuth.InvalidGrant,
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
                    Code = Errors.OAuth.InvalidGrant,
                    Description = "Invalid authorization code."
                });
        }

        var device = authorizationCode.Device;
        var session = await _sessionManager.FindAsync(device, cancellationToken);
        var user = await _userManager.FindByIdAsync(device.UserId, cancellationToken);
        if (session is null || session.ExpireDate < DateTimeOffset.UtcNow || user is null)
        {
            return Results.BadRequest(new Error()
            {
                Code = Errors.OAuth.InvalidGrant,
                Description = "Invalid authorization code."
            });
        }

        var codeResult = await _authorizationCodeManager.UseAsync(authorizationCode, cancellationToken);
        if (!codeResult.Succeeded) return codeResult;

        var accessClaims = _claimBuilderFactory.CreateAccessBuilder()
            .WithIssuer(_options.Issuer)
            .WithAudience(client.Audience)
            .WithSubject(user.Id.ToString())
            .WithTokenId(Guid.CreateVersion7().ToString())
            .WithSessionId(session.Id.ToString())
            .WithIssuedTime(DateTimeOffset.UtcNow)
            .WithExpirationTime(DateTimeOffset.UtcNow.Add(_options.IdTokenLifetime))
            .WithNonce(authorizationCode.Nonce)
            .WithScope(client.AllowedScopes.Select(x => x.Scope.Name))
            .Build();

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
            var hasher = _hasherFactory.CreateHasher(HashAlgorithm.Sha512);
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
                ExpiredDate = DateTimeOffset.UtcNow.Add(client.RefreshTokenLifetime),
                CreateDate = DateTimeOffset.UtcNow
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
            var scopes = client.AllowedScopes.Select(x => x.Scope.Name).ToList();
            var idClaims = _claimBuilderFactory.CreateIdBuilder()
                .WithOpenId(user, scopes)
                .WithIssuer(_options.Issuer)
                .WithAudience(client.Audience)
                .WithSubject(user.Id.ToString())
                .WithTokenId(Guid.CreateVersion7().ToString())
                .WithSessionId(session.Id.ToString())
                .WithNonce(authorizationCode.Nonce)
                .WithIssuedTime(DateTimeOffset.UtcNow)
                .WithAuthenticationTime(DateTimeOffset.UtcNow)
                .WithExpirationTime(DateTimeOffset.UtcNow.Add(_options.IdTokenLifetime))
                .Build();

            var idTokenContext = new JwtTokenContext { Claims = idClaims };
            response.IdToken = await jwtTokenFactory.CreateTokenAsync(idTokenContext, cancellationToken);
        }

        return Results.Ok(response);
    }
}