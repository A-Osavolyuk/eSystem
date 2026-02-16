using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Constants;
using eSecurity.Server.Security.Cryptography.Pkce;
using eSecurity.Server.Security.Cryptography.Tokens;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
using eSystem.Core.Security.Authentication.OpenIdConnect.Client;
using eSystem.Core.Security.Authorization.OAuth.Token.AuthorizationCode;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.AuthorizationCode;

public class OAuthAuthorizationCodeFlow(
    IUserManager userManager,
    IPkceHandler pkceHandler,
    IClientManager clientManager,
    IAuthorizationCodeManager authorizationCodeManager,
    ITokenFactoryProvider tokenFactoryProvider,
    IOptions<TokenConfigurations> options) : IAuthorizationCodeFlow
{
    private readonly IUserManager _userManager = userManager;
    private readonly IPkceHandler _pkceHandler = pkceHandler;
    private readonly IClientManager _clientManager = clientManager;
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

        if (client.Id != code.ClientId || !client.HasUri(context.RedirectUri, UriType.Redirect))
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Invalid authorization code."
            });
        }

        if (!client.HasGrantType(context.GrantType))
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.UnsupportedGrantType,
                Description = $"'{context.GrantType}' grant is not supported by client."
            });

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

            var isValidPkce = _pkceHandler.Verify(
                code.CodeChallenge,
                code.CodeChallengeMethod,
                context.CodeVerifier
            );

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

        var response = new AuthorizationCodeResponse()
        {
            ExpiresIn = (int)_tokenConfigurations.DefaultAccessTokenLifetime.TotalSeconds,
            TokenType = ResponseTokenTypes.Bearer,
        };

        var accessTokenFactory = _tokenFactoryProvider.GetFactory(TokenType.AccessToken);
        var accessTokenResult = await accessTokenFactory.CreateAsync(client, user, cancellationToken: cancellationToken);
        if (!accessTokenResult.Succeeded)
        {
            var error = accessTokenResult.GetError();
            return Results.InternalServerError(error);
        }
        
        if (!accessTokenResult.TryGetValue(out var accessToken))
        {
            return Results.InternalServerError(new Error()
            {
                Code = ErrorTypes.OAuth.ServerError,
                Description = "Server error"
            });
        }
        
        response.AccessToken = accessToken;

        if (client.AllowOfflineAccess)
        {
            var refreshTokenFactory = _tokenFactoryProvider.GetFactory(TokenType.RefreshToken);
            var refreshTokenResult = await refreshTokenFactory.CreateAsync(client, user, cancellationToken: cancellationToken);
            if (!refreshTokenResult.Succeeded)
            {
                var error = refreshTokenResult.GetError();
                return Results.InternalServerError(error);
            }
            
            if (!refreshTokenResult.TryGetValue(out var refreshToken))
            {
                return Results.InternalServerError(new Error()
                {
                    Code = ErrorTypes.OAuth.ServerError,
                    Description = "Server error"
                });
            }
            
            response.RefreshToken = refreshToken;
        }

        return Results.Ok(response);
    }
}