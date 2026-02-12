using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Constants;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Server.Security.Cryptography.Pkce;
using eSecurity.Server.Security.Cryptography.Tokens;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authentication.OpenIdConnect.Client;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;
using eSystem.Core.Security.Authorization.OAuth.Token.AuthorizationCode;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.AuthorizationCode;

public class OidcAuthorizationCodeFlow(
    IUserManager userManager,
    IPkceHandler pkceHandler,
    IClientManager clientManager,
    ISessionManager sessionManager,
    IAuthorizationCodeManager authorizationCodeManager,
    ITokenFactoryProvider tokenFactoryProvider,
    IOptions<TokenConfigurations> options) : IAuthorizationCodeFlow
{
    private readonly IClientManager _clientManager = clientManager;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly IUserManager _userManager = userManager;
    private readonly IPkceHandler _pkceHandler = pkceHandler;
    private readonly IAuthorizationCodeManager _authorizationCodeManager = authorizationCodeManager;
    private readonly ITokenFactoryProvider _tokenFactoryProvider = tokenFactoryProvider;
    private readonly TokenConfigurations _tokenConfigurations = options.Value;

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
            ExpiresIn = (int)_tokenConfigurations.DefaultAccessTokenLifetime.TotalSeconds,
            TokenType = ResponseTokenTypes.Bearer,
        };
        
        var session = await _sessionManager.FindAsync(user, cancellationToken);
        if (session is null || session.ExpireDate < DateTimeOffset.UtcNow)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Invalid authorization code."
            });
        }

        var accessTokenFactory = _tokenFactoryProvider.GetFactory(TokenType.AccessToken);
        var accessTokenResult = await accessTokenFactory.CreateAsync(client, user, 
            session, cancellationToken: cancellationToken);
        
        if (!accessTokenResult.IsSucceeded) 
            return Results.InternalServerError(accessTokenResult.Error!);
        
        response.AccessToken = accessTokenResult.Token;

        var clientResult = await _clientManager.RelateAsync(client, session, cancellationToken);
        if (!clientResult.Succeeded) return clientResult;

        if (client.AllowOfflineAccess && client.HasScope(ScopeTypes.OfflineAccess))
        {
            var refreshTokenFactory = _tokenFactoryProvider.GetFactory(TokenType.RefreshToken);
            var refreshTokenResult = await refreshTokenFactory.CreateAsync(client, user, 
                session, cancellationToken: cancellationToken);
        
            if (!refreshTokenResult.IsSucceeded) 
                return Results.InternalServerError(refreshTokenResult.Error!);
            
            response.RefreshToken = refreshTokenResult.Token;
        }

        var idTokenFactory = _tokenFactoryProvider.GetFactory(TokenType.IdToken);
        var idTokenResult = await idTokenFactory.CreateAsync(client, user, session, cancellationToken: cancellationToken);
        if (!idTokenResult.IsSucceeded) 
            return Results.InternalServerError(idTokenResult.Error!);
            
        response.IdToken = idTokenResult.Token;
        
        return Results.Ok(response);
    }
}