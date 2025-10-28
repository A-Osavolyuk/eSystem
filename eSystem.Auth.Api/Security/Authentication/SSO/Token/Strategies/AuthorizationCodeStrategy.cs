using System.Security.Claims;
using eSystem.Auth.Api.Security.Authentication.SSO.Code;
using eSystem.Auth.Api.Security.Authentication.SSO.PKCE;
using eSystem.Auth.Api.Security.Authentication.SSO.Session;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Responses.Auth;
using eSystem.Core.Security.Authentication.JWT;
using eSystem.Core.Security.Authentication.SSO.Client;
using eSystem.Core.Security.Claims;
using eSystem.Core.Security.Cryptography.Protection;
using eSystem.Core.Security.Cryptography.Tokens;

namespace eSystem.Auth.Api.Security.Authentication.SSO.Token.Strategies;

public class AuthorizationCodeStrategy(
    IUserManager userManager,
    ISessionManager sessionManager,
    IAuthorizationCodeManager authorizationCodeManager,
    ITokenManager tokenManager,
    IPkceHandler pkceHandler,
    IProtectorFactory protectorFactory,
    IOptions<JwtOptions> options) : TokenStrategy
{
    private readonly IUserManager userManager = userManager;
    private readonly ISessionManager sessionManager = sessionManager;
    private readonly IAuthorizationCodeManager authorizationCodeManager = authorizationCodeManager;
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly IPkceHandler pkceHandler = pkceHandler;
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
        if (!authorizationCode.RedirectUri.Equals(redirectUri))
            return Results.BadRequest("Invalid redirect URI.");

        var client = authorizationCode.Client;
        if (!client.ClientId.Equals(request.ClientId)) return Results.BadRequest("Invalid client ID.");
        if (!client.HasUri(request.RedirectUri)) return Results.BadRequest("Invalid redirect URI.");

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
            Token = tokenManager.GenerateRefreshToken(),
            ExpireDate = DateTimeOffset.UtcNow.AddDays(options.RefreshTokenExpirationDays),
            CreateDate = DateTimeOffset.UtcNow
        };
        
        var tokenResult = await tokenManager.CreateAsync(refreshToken, cancellationToken);
        if (!tokenResult.Succeeded) return tokenResult;
        
        var protector = protectorFactory.Create(ProtectionPurposes.RefreshToken);
        var protectedRefreshToken = protector.Protect(refreshToken.Token);
        var scopes = string.Join(' ', client.AllowedScopes.Select(x => x.Scope.Name));
        var claims = new List<Claim>()
        {
            new(AppClaimTypes.Jti, Guid.CreateVersion7().ToString()),
            new(AppClaimTypes.Sub, user.Id.ToString()),
            new(AppClaimTypes.Nonce, authorizationCode.Nonce),
            new(AppClaimTypes.Scope, scopes)
        };
        
        var accessToken = tokenManager.GenerateAccessToken(claims, client.Name);
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