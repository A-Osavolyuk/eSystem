using eSecurity.Core.Common.Responses;
using eSecurity.Core.Security.Cryptography.Protection.Constants;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.Oidc.Client;
using eSecurity.Server.Security.Authentication.Oidc.Code;
using eSecurity.Server.Security.Authentication.Oidc.Pkce;
using eSecurity.Server.Security.Authentication.Oidc.Session;
using eSecurity.Server.Security.Cryptography.Keys;
using eSecurity.Server.Security.Cryptography.Tokens;
using eSecurity.Server.Security.Cryptography.Tokens.Jwt;
using eSecurity.Server.Security.Identity.Claims;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Security.Authentication.Oidc.Constants;
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
    ITokenFactory tokenFactory,
    ITokenManager tokenManager,
    IPkceHandler pkceHandler,
    IKeyFactory keyFactory,
    IDataProtectionProvider protectionProvider,
    IClaimBuilderFactory claimBuilderFactory,
    IOptions<TokenOptions> options) : ITokenStrategy
{
    private readonly IUserManager _userManager = userManager;
    private readonly IClientManager _clientManager = clientManager;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly IAuthorizationCodeManager _authorizationCodeManager = authorizationCodeManager;
    private readonly ITokenFactory _tokenFactory = tokenFactory;
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly IPkceHandler _pkceHandler = pkceHandler;
    private readonly IKeyFactory _keyFactory = keyFactory;
    private readonly IDataProtectionProvider _protectionProvider = protectionProvider;
    private readonly IClaimBuilderFactory _claimBuilderFactory = claimBuilderFactory;
    private readonly TokenOptions _options = options.Value;

    public async ValueTask<Result> ExecuteAsync(TokenPayload payload,
        CancellationToken cancellationToken = default)
    {
        if (payload is not AuthorizationCodeTokenPayload authorizationPayload)
            throw new NotSupportedException("Payload type must be 'AuthorizationCodeTokenPayload'");

        var client = await _clientManager.FindByIdAsync(authorizationPayload.ClientId, cancellationToken);
        if (client is null) return Results.NotFound("Client was not found.");
        if (!client.HasGrantType(authorizationPayload.GrantType))
            return Results.BadRequest($"Client doesn't support grant type {authorizationPayload.GrantType}");
        
        var code = authorizationPayload.Code!;
        var authorizationCode = await _authorizationCodeManager.FindByCodeAsync(code, cancellationToken);
        if (authorizationCode is null) return Results.NotFound("Authorization code not found.");
        if (authorizationCode.Used) return Results.BadRequest("Authorization code has already been used.");
        if (authorizationCode.ExpireDate < DateTimeOffset.UtcNow)
            return Results.BadRequest("Authorization code has expired.");

        var redirectUri = authorizationPayload.RedirectUri;
        if (string.IsNullOrEmpty(redirectUri) || !authorizationCode.RedirectUri.Equals(redirectUri))
            return Results.BadRequest("Invalid redirect URI.");
        
        if (client.Id != authorizationCode.ClientId) return Results.BadRequest("Invalid client ID.");
        if (!client.HasRedirectUri(redirectUri)) return Results.BadRequest("Invalid redirect URI.");


        if (client is { Type: ClientType.Confidential, RequireClientSecret: true })
        {
            if (string.IsNullOrEmpty(authorizationPayload.ClientSecret))
                return Results.BadRequest("Client secret is required.");

            if (!client.Secret.Equals(authorizationPayload.ClientSecret))
                return Results.BadRequest("Invalid client secret.");
        }

        if (client is { Type: ClientType.Public, RequirePkce: true })
        {
            if (string.IsNullOrWhiteSpace(authorizationCode.CodeChallenge))
                return Results.BadRequest("Missing code challenge in authorization code.");

            if (string.IsNullOrWhiteSpace(authorizationCode.CodeChallengeMethod))
                return Results.BadRequest("Missing code challenge method in authorization code.");

            if (string.IsNullOrWhiteSpace(authorizationPayload.CodeVerifier))
                return Results.BadRequest("Missing code verifier in token request.");

            var isValidPkce = _pkceHandler.Verify(
                authorizationCode.CodeChallenge,
                authorizationCode.CodeChallengeMethod,
                authorizationPayload.CodeVerifier
            );

            if (!isValidPkce) return Results.BadRequest("Invalid PKCE verification (code verifier mismatch).");
        }

        var device = authorizationCode.Device;
        var session = await _sessionManager.FindAsync(device, cancellationToken);
        if (session is null) return Results.NotFound("Session not found.");
        if (session.ExpireDate < DateTimeOffset.UtcNow) return Results.BadRequest("Session has expired.");

        var user = await _userManager.FindByIdAsync(device.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

        var codeResult = await _authorizationCodeManager.UseAsync(authorizationCode, cancellationToken);
        if (!codeResult.Succeeded) return codeResult;

        var accessClaims = _claimBuilderFactory.CreateAccessBuilder()
            .WithIssuer(_options.Issuer)
            .WithAudience(client.Name)
            .WithSubject(user.Id.ToString())
            .WithTokenId(Guid.CreateVersion7().ToString())
            .WithSessionId(session.Id.ToString())
            .WithIssuedTime(DateTimeOffset.UtcNow)
            .WithExpirationTime(DateTimeOffset.UtcNow.Add(_options.AccessTokenLifetime))
            .WithNonce(authorizationCode.Nonce)
            .WithScope(client.AllowedScopes.Select(x => x.Scope.Name))
            .Build();
        
        var response = new TokenResponse()
        {
            AccessToken = await _tokenFactory.CreateAsync(accessClaims, cancellationToken),
            ExpiresIn = (int)_options.AccessTokenLifetime.TotalSeconds,
            TokenType = TokenTypes.Bearer,
        };
        
        if (client.AllowOfflineAccess && client.HasScope(Scopes.OfflineAccess))
        {
            var refreshToken = new RefreshTokenEntity()
            {
                Id = Guid.CreateVersion7(),
                ClientId = client.Id,
                SessionId = session.Id,
                Token = _keyFactory.Create(20),
                ExpireDate = DateTimeOffset.UtcNow.Add(client.RefreshTokenLifetime),
                CreateDate = DateTimeOffset.UtcNow
            };
        
            var tokenResult = await _tokenManager.CreateAsync(refreshToken, cancellationToken);
            if (!tokenResult.Succeeded) return tokenResult;
            
            var protector = _protectionProvider.CreateProtector(ProtectionPurposes.RefreshToken);
            var protectedRefreshToken = protector.Protect(refreshToken.Token);
            response.RefreshToken = protectedRefreshToken;
        }

        if (client.HasScope(Scopes.OpenId))
        {
            var idClaims = _claimBuilderFactory.CreateIdBuilder()
                .WithOpenId(user, client)
                .WithIssuer(_options.Issuer)
                .WithAudience(client.Name)
                .WithSubject(user.Id.ToString())
                .WithTokenId(Guid.CreateVersion7().ToString())
                .WithSessionId(session.Id.ToString())
                .WithNonce(authorizationCode.Nonce)
                .WithIssuedTime(DateTimeOffset.UtcNow)
                .WithAuthenticationTime(DateTimeOffset.UtcNow)
                .WithExpirationTime(DateTimeOffset.UtcNow.Add(_options.IdTokenLifetime))
                .Build();
            
            response.IdToken = await _tokenFactory.CreateAsync(idClaims, cancellationToken);
        }

        return Result.Success(response);
    }
}