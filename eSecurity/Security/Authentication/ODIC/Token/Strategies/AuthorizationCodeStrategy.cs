using eSecurity.Data.Entities;
using eSecurity.Security.Authentication.JWT;
using eSecurity.Security.Authentication.JWT.Management;
using eSecurity.Security.Authentication.JWT.Payloads;
using eSecurity.Security.Authentication.ODIC.Code;
using eSecurity.Security.Authentication.ODIC.PKCE;
using eSecurity.Security.Authentication.ODIC.Session;
using eSecurity.Security.Identity.User;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Responses.Auth;
using eSystem.Core.Security.Authentication.JWT;
using eSystem.Core.Security.Authentication.ODIC.Client;
using eSystem.Core.Security.Authentication.ODIC.Constants;
using eSystem.Core.Security.Cryptography.Keys;
using eSystem.Core.Security.Cryptography.Protection;
using eSystem.Core.Security.Cryptography.Tokens;
using eSystem.Core.Security.Cryptography.Tokens.Constants;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Security.Authentication.ODIC.Token.Strategies;

public class AuthorizationCodeStrategy(
    IUserManager userManager,
    ISessionManager sessionManager,
    IAuthorizationCodeManager authorizationCodeManager,
    ITokenFactoryResolver tokenFactoryResolver,
    ITokenManager tokenManager,
    IPkceHandler pkceHandler,
    IKeyFactory keyFactory,
    IProtectorFactory protectorFactory,
    IOptions<JwtOptions> options) : TokenStrategy
{
    private readonly IUserManager userManager = userManager;
    private readonly ISessionManager sessionManager = sessionManager;
    private readonly IAuthorizationCodeManager authorizationCodeManager = authorizationCodeManager;
    private readonly ITokenFactoryResolver tokenFactoryResolver = tokenFactoryResolver;
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly IPkceHandler pkceHandler = pkceHandler;
    private readonly IKeyFactory keyFactory = keyFactory;
    private readonly IProtectorFactory protectorFactory = protectorFactory;
    private readonly JwtOptions options = options.Value;

    public override async ValueTask<Result> HandleAsync(TokenRequest request,
        CancellationToken cancellationToken = default)
    {
        var code = request.Code!;
        var authorizationCode = await authorizationCodeManager.FindByCodeAsync(code, cancellationToken);
        if (authorizationCode is null) return Results.NotFound("Authorization code not found.");
        if (authorizationCode.Used) return Results.BadRequest("Authorization code has already been used.");
        if (authorizationCode.ExpireDate < DateTimeOffset.UtcNow)
            return Results.BadRequest("Authorization code has expired.");

        var redirectUri = request.RedirectUri;
        if (string.IsNullOrEmpty(redirectUri) || !authorizationCode.RedirectUri.Equals(redirectUri))
            return Results.BadRequest("Invalid redirect URI.");

        var client = authorizationCode.Client;
        if (!client.ClientId.Equals(request.ClientId)) return Results.BadRequest("Invalid client ID.");
        if (!client.HasRedirectUri(redirectUri)) return Results.BadRequest("Invalid redirect URI.");

        if (client is { Type: ClientType.Confidential, RequireClientSecret: true })
        {
            if (string.IsNullOrEmpty(request.ClientSecret))
                return Results.BadRequest("Client secret is required.");

            if (!client.ClientSecret.Equals(request.ClientSecret))
                return Results.BadRequest("Invalid client secret.");
        }

        if (client is { Type: ClientType.Public, RequirePkce: true })
        {
            if (string.IsNullOrWhiteSpace(authorizationCode.CodeChallenge))
                return Results.BadRequest("Missing code challenge in authorization code.");

            if (string.IsNullOrWhiteSpace(authorizationCode.CodeChallengeMethod))
                return Results.BadRequest("Missing code challenge method in authorization code.");

            if (string.IsNullOrWhiteSpace(request.CodeVerifier))
                return Results.BadRequest("Missing code verifier in token request.");

            var isValidPkce = pkceHandler.Verify(
                authorizationCode.CodeChallenge,
                authorizationCode.CodeChallengeMethod,
                request.CodeVerifier
            );

            if (!isValidPkce) return Results.BadRequest("Invalid PKCE verification (code verifier mismatch).");
        }

        var device = authorizationCode.Device;
        var session = await sessionManager.FindAsync(device, cancellationToken);
        if (session is null) return Results.NotFound("Session not found.");
        if (session.ExpireDate < DateTimeOffset.UtcNow) return Results.BadRequest("Session has expired.");

        var user = await userManager.FindByIdAsync(device.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

        //TODO: Implement ID token generation

        var codeResult = await authorizationCodeManager.UseAsync(authorizationCode, cancellationToken);
        if (!codeResult.Succeeded) return codeResult;
        
        var refreshToken = new RefreshTokenEntity()
        {
            Id = Guid.CreateVersion7(),
            ClientId = client.Id,
            SessionId = session.Id,
            Token = keyFactory.Create(20),
            ExpireDate = DateTimeOffset.UtcNow.AddDays(options.RefreshTokenExpirationDays),
            CreateDate = DateTimeOffset.UtcNow
        };
        
        var tokenResult = await tokenManager.CreateAsync(refreshToken, cancellationToken);
        if (!tokenResult.Succeeded) return tokenResult;

        var accessTokenFactory = tokenFactoryResolver.Create(JwtTokenType.AccessToken);
        var accessTokenPayload = new AccessTokenPayload()
        {
            Subject = user.Id.ToString(),
            Audience = client.ClientId,
            Nonce = authorizationCode.Nonce,
            Scopes = client.AllowedScopes.Select(x => x.Scope.Name).ToList(),
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(options.AccessTokenExpirationMinutes),
            IssuedAt = DateTimeOffset.UtcNow,
        };
        
        var accessToken = accessTokenFactory.Create(accessTokenPayload);
        var protector = protectorFactory.Create(ProtectionPurposes.RefreshToken);
        var protectedRefreshToken = protector.Protect(refreshToken.Token);
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