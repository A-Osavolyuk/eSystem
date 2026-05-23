using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Idp.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Idp.Security.Cryptography.Pkce;
using eSecurity.Idp.Security.Cryptography.Tokens;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Security.Authentication.OpenIdConnect.Client;
using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Server.Security.Authorization.OAuth.Token.AuthorizationCode;

namespace eSecurity.Idp.Security.Authorization.OAuth.Token.AuthorizationCode;

public class OidcAuthorizationCodeFlow(
    IUserManager userManager,
    IPkceHandler pkceHandler,
    IClientManager clientManager,
    IAuthorizationCodeManager authorizationCodeManager,
    ITokenFactoryProvider tokenFactoryProvider,
    IOptions<TokenConfigurations> options) : IAuthorizationCodeFlow
{
    private readonly IClientManager _clientManager = clientManager;
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
        {
            return Results.ClientError(ClientErrorCode.Unauthorized, new Error
            {
                Code = ErrorCode.InvalidClient,
                Description = "Client was not found."
            });
        }

        if (client.Id != code.ClientId || !client.HasUri(context.RedirectUri!, UriType.Redirect))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidGrant,
                Description = "Invalid authorization code."
            });
        }

        if (!client.HasGrantType(context.GrantType))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.UnsupportedGrantType,
                Description = $"'{context.GrantType}' grant is not supported by client."
            });
        }

        var user = await _userManager.FindByIdAsync(code.UserId, cancellationToken);
        if (user is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidGrant,
                Description = "Invalid authorization code."
            });
        }

        if (client is { ClientType: ClientType.Public, RequirePkce: true })
        {
            if (string.IsNullOrWhiteSpace(code.CodeChallenge) || 
                code.CodeChallengeMethod is null || 
                string.IsNullOrWhiteSpace(context.CodeVerifier))
            {
                return Results.ClientError(ClientErrorCode.BadRequest, new Error
                {
                    Code = ErrorCode.InvalidGrant,
                    Description = "Invalid authorization code."
                });
            }

            var isValidPkce = _pkceHandler.Verify(
                code.CodeChallenge,
                code.CodeChallengeMethod.Value,
                context.CodeVerifier
            );
            
            if (!isValidPkce)
            {
                return Results.ClientError(ClientErrorCode.BadRequest, new Error
                {
                    Code = ErrorCode.InvalidGrant,
                    Description = "Invalid authorization code."
                });
            }
        }

        var codeResult = await _authorizationCodeManager.UseAsync(code, cancellationToken);
        if (!codeResult.Succeeded) return codeResult;

        var response = new AuthorizationCodeResponse
        {
            ExpiresIn = (int)_tokenConfigurations.DefaultAccessTokenLifetime.TotalSeconds,
            TokenType = ResponseTokenType.Bearer,
        };

        var session = code.Session;
        if (session.ExpireDate < DateTimeOffset.UtcNow)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidGrant,
                Description = "Invalid authorization code."
            });
        }

        var accessTokenFactory = _tokenFactoryProvider.GetFactory(TokenType.AccessToken);
        var accessTokenResult = await accessTokenFactory.CreateAsync(client, user,
            session, cancellationToken: cancellationToken);

        if (!accessTokenResult.Succeeded)
        {
            var error = accessTokenResult.GetError();
            return Results.ServerError(ServerErrorCode.InternalServerError, error);
        }

        if (!accessTokenResult.TryGetValue(out var accessToken))
        {
            return Results.ServerError(ServerErrorCode.InternalServerError, new Error()
            {
                Code = ErrorCode.ServerError,
                Description = "Server error"
            });
        }

        response.AccessToken = accessToken;

        var clientResult = await _clientManager.RelateAsync(client, session, cancellationToken);
        if (!clientResult.Succeeded) return clientResult;

        if (client.AllowOfflineAccess && client.HasScope(ScopeTypes.OfflineAccess))
        {
            var refreshTokenFactory = _tokenFactoryProvider.GetFactory(TokenType.RefreshToken);
            var refreshTokenResult = await refreshTokenFactory.CreateAsync(client, user,
                session, cancellationToken: cancellationToken);

            if (!refreshTokenResult.TryGetValue(out var refreshToken))
            {
                return Results.ServerError(ServerErrorCode.InternalServerError, new Error()
                {
                    Code = ErrorCode.ServerError,
                    Description = "Server error"
                });
            }

            response.RefreshToken = refreshToken;
        }

        var idTokenFactory = _tokenFactoryProvider.GetFactory(TokenType.IdToken);
        var idFactoryOptions = new TokenFactoryOptions() { Nonce = code.Nonce };
        var idTokenResult = await idTokenFactory.CreateAsync(client, user, 
            session, idFactoryOptions, cancellationToken);
        
        if (!idTokenResult.Succeeded)
        {
            var error = idTokenResult.GetError();
            return Results.ServerError(ServerErrorCode.InternalServerError, error);
        }

        if (!idTokenResult.TryGetValue(out var idToken))
        {
            return Results.ServerError(ServerErrorCode.InternalServerError, new Error()
            {
                Code = ErrorCode.ServerError,
                Description = "Server error"
            });
        }

        response.IdToken = idToken;

        return Results.Success(SuccessCodes.Ok, response);
    }
}