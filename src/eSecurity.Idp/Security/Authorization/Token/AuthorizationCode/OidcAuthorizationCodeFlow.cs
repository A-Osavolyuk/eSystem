using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.Client;
using eSecurity.Idp.Security.Cryptography.Pkce;
using eSecurity.Idp.Security.Cryptography.Tokens;
using eSecurity.Idp.Security.Cryptography.Tokens.Access;
using eSecurity.Idp.Security.Cryptography.Tokens.Id;
using eSecurity.Idp.Security.Cryptography.Tokens.Refresh;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Security.Authentication.OpenIdConnect.Client;
using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Server.Security.Authorization.OAuth.Token.AuthorizationCode;

namespace eSecurity.Idp.Security.Authorization.Token.AuthorizationCode;

public class OidcAuthorizationCodeFlow(
    IUserQueryService userQueryService,
    IPkceHandler pkceHandler,
    IAuthorizationCodeManager authorizationCodeManager,
    ITokenFactoryProvider tokenFactoryProvider,
    IClientQueryService clientQueryService,
    IClientCommandService clientCommandService,
    IOptions<TokenConfigurations> options) : IAuthorizationCodeFlow
{
    private readonly IUserQueryService _userQueryService = userQueryService;
    private readonly IPkceHandler _pkceHandler = pkceHandler;
    private readonly IAuthorizationCodeManager _authorizationCodeManager = authorizationCodeManager;
    private readonly ITokenFactoryProvider _tokenFactoryProvider = tokenFactoryProvider;
    private readonly IClientQueryService _clientQueryService = clientQueryService;
    private readonly IClientCommandService _clientCommandService = clientCommandService;
    private readonly TokenConfigurations _tokenConfigurations = options.Value;

    public async ValueTask<Result> ExecuteAsync(AuthorizationCodeEntity code,
        AuthorizationCodeFlowContext context, CancellationToken cancellationToken = default)
    {
        var client = await _clientQueryService.GetByIdAsync(context.ClientId, cancellationToken);
        if (client is null)
        {
            return Results.ClientError(ClientErrorCode.Unauthorized, new Error
            {
                Code = ErrorCode.InvalidClient,
                Description = "Client was not found."
            });
        }

        var clientUris = await _clientQueryService.GetUrisAsync(client.Id, cancellationToken);
        if (client.Id != code.ClientId || 
            clientUris.All(x => x.Type != UriType.Redirect && x.Uri != context.RedirectUri))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidGrant,
                Description = "Invalid authorization code."
            });
        }

        var clientGrantTypes = await _clientQueryService.GetSupportedGrantTypesAsync(client.Id, cancellationToken);
        if (clientGrantTypes.All(x => x.Grant.Grant != context.GrantType))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.UnsupportedGrantType,
                Description = $"'{context.GrantType}' grant is not supported by client."
            });
        }

        var user = await _userQueryService.GetByIdAsync(code.UserId, cancellationToken);
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

        var accessTokenFactoryContext = new AccessTokenFactoryContext
        {
            ClientId = client.Id,
            TokenLifetime = client.AccessTokenLifetime,
            UserId = user.Id,
            SessionId = session.Id,
            TokenType = client.AccessTokenType
        };
            
        var accessTokenFactory = _tokenFactoryProvider.GetFactory<AccessTokenFactoryContext>();
        var accessTokenResult = await accessTokenFactory.CreateAsync(accessTokenFactoryContext, 
            cancellationToken: cancellationToken);

        if (!accessTokenResult.Succeeded)
        {
            var error = accessTokenResult.GetError();
            return Results.ServerError(ServerErrorCode.InternalServerError, error);
        }

        if (!accessTokenResult.TryGetValue(out var accessToken))
        {
            return Results.ServerError(ServerErrorCode.InternalServerError, new Error
            {
                Code = ErrorCode.ServerError,
                Description = "Server error"
            });
        }

        response.AccessToken = accessToken;

        var clientResult = await _clientCommandService.RelateAsync(client.Id, session.Id, cancellationToken);
        if (!clientResult.Succeeded) return clientResult;

        var clientScopes = await _clientQueryService.GetAllowedScopesAsync(client.Id, cancellationToken);
        if (client.AllowOfflineAccess && clientScopes.Any(x => x.Scope.Value == ScopeTypes.OfflineAccess))
        {
            var refreshTokenFactoryContext = new RefreshTokenFactoryContext
            {
                ClientId = client.Id,
                TokenLifetime = client.RefreshTokenLifetime,
                UserId = user.Id,
                SessionId = session.Id
            };
                
            var refreshTokenFactory = _tokenFactoryProvider.GetFactory<RefreshTokenFactoryContext>();
            var refreshTokenResult = await refreshTokenFactory.CreateAsync(refreshTokenFactoryContext, 
                cancellationToken: cancellationToken);

            if (!refreshTokenResult.TryGetValue(out var refreshToken))
            {
                return Results.ServerError(ServerErrorCode.InternalServerError, new Error
                {
                    Code = ErrorCode.ServerError,
                    Description = "Server error"
                });
            }

            response.RefreshToken = refreshToken;
        }

        var idTokenFactoryContext = new IdTokenFactoryContext
        {
            ClientId = client.Id,
            UserId = user.Id,
            SessionId = session.Id,
            TokenLifetime = client.IdTokenLifetime
        };
        
        var idTokenFactory = _tokenFactoryProvider.GetFactory<IdTokenFactoryContext>();
        var idFactoryOptions = new TokenFactoryOptions { Nonce = code.Nonce };
        var idTokenResult = await idTokenFactory.CreateAsync(idTokenFactoryContext, idFactoryOptions, cancellationToken);
        
        if (!idTokenResult.Succeeded)
        {
            var error = idTokenResult.GetError();
            return Results.ServerError(ServerErrorCode.InternalServerError, error);
        }

        if (!idTokenResult.TryGetValue(out var idToken))
        {
            return Results.ServerError(ServerErrorCode.InternalServerError, new Error
            {
                Code = ErrorCode.ServerError,
                Description = "Server error"
            });
        }

        response.IdToken = idToken;

        return Results.Success(SuccessCodes.Ok, response);
    }
}