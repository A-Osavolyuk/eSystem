using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.Client;
using eSecurity.Idp.Security.Cryptography.Pkce;
using eSecurity.Idp.Security.Cryptography.Tokens;
using eSecurity.Idp.Security.Cryptography.Tokens.Access;
using eSecurity.Idp.Security.Cryptography.Tokens.Refresh;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authentication.OpenIdConnect.Client;
using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Server.Security.Authorization.OAuth.Token.AuthorizationCode;

namespace eSecurity.Idp.Security.Authorization.Token.AuthorizationCode;

public class OAuthAuthorizationCodeFlow(
    IUserQueryService userQueryService,
    IPkceHandler pkceHandler,
    IAuthorizationCodeManager authorizationCodeManager,
    ITokenFactoryProvider tokenFactoryProvider,
    IClientQueryService clientQueryService,
    IOptions<TokenConfigurations> options) : IAuthorizationCodeFlow
{
    private readonly IUserQueryService _userQueryService = userQueryService;
    private readonly IPkceHandler _pkceHandler = pkceHandler;
    private readonly IAuthorizationCodeManager _authorizationCodeManager = authorizationCodeManager;
    private readonly ITokenFactoryProvider _tokenFactoryProvider = tokenFactoryProvider;
    private readonly IClientQueryService _clientQueryService = clientQueryService;
    private readonly TokenConfigurations _tokenConfigurations = options.Value;

    public async ValueTask<Result> ExecuteAsync(AuthorizationCodeEntity code, 
        AuthorizationCodeFlowContext context, CancellationToken cancellationToken = default)
    {
        var client = await _clientQueryService.GetByIdAsync(context.ClientId, cancellationToken);
        if (client is null)
            return Results.ClientError(ClientErrorCode.Unauthorized, new Error
            {
                Code = ErrorCode.InvalidClient,
                Description = "Client was not found."
            });

        var clientUris = await _clientQueryService.GetUrisAsync(client, cancellationToken);
        if (client.Id != code.ClientId || 
            clientUris.All(x => x.Type != UriType.Redirect && x.Uri != context.RedirectUri))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidGrant,
                Description = "Invalid authorization code."
            });
        }

        var clientGrantTypes = await _clientQueryService.GetSupportedGrantTypesAsync(
            client, cancellationToken);
        
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

        var accessTokenFactoryContext = new AccessTokenFactoryContext
        {
            Client = client,
            User = user
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

        if (client.AllowOfflineAccess)
        {
            var refreshTokenFactoryContext = new RefreshTokenFactoryContext
            {
                Client = client,
                User = user
            };
                
            var refreshTokenFactory = _tokenFactoryProvider.GetFactory<RefreshTokenFactoryContext>();
            var refreshTokenResult = await refreshTokenFactory.CreateAsync(refreshTokenFactoryContext, 
                cancellationToken: cancellationToken);
            
            if (!refreshTokenResult.Succeeded)
            {
                var error = refreshTokenResult.GetError();
                return Results.ServerError(ServerErrorCode.InternalServerError, error);
            }
            
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

        return Results.Success(SuccessCodes.Ok, response);
    }
}