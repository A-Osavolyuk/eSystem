using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.Client;
using eSecurity.Idp.Security.Authentication.Session;
using eSecurity.Idp.Security.Cryptography.Tokens;
using eSecurity.Idp.Security.Cryptography.Tokens.Access;
using eSecurity.Idp.Security.Cryptography.Tokens.Id;
using eSecurity.Idp.Security.Cryptography.Tokens.Refresh;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Server.Security.Authorization.OAuth.Token.RefreshToken;

namespace eSecurity.Idp.Security.Authorization.Token.RefreshToken;

public sealed class OidcRefreshTokenFlow(
    IUserQueryService userQueryService,
    ITokenFactoryProvider tokenFactoryProvider,
    IClientQueryService clientQueryService,
    ISessionCommandService sessionCommandService,
    ISessionQueryService sessionQueryService,
    IOptions<TokenConfigurations> options) : IRefreshTokenFlow
{
    private readonly IUserQueryService _userQueryService = userQueryService;
    private readonly ITokenFactoryProvider _tokenFactoryProvider = tokenFactoryProvider;
    private readonly IClientQueryService _clientQueryService = clientQueryService;
    private readonly ISessionCommandService _sessionCommandService = sessionCommandService;
    private readonly ISessionQueryService _sessionQueryService = sessionQueryService;
    private readonly IOptions<TokenConfigurations> _options = options;
    private readonly TokenConfigurations _tokenConfigurations = options.Value;

    public async ValueTask<Result> ExecuteAsync(OpaqueTokenEntity token, RefreshTokenFlowContext flowContext,
        CancellationToken cancellationToken = default)
    {
        if (!token.SessionId.HasValue)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.InvalidGrant,
                Description = "Invalid refresh token."
            });
        }

        var session = await _sessionQueryService.GetByIdAsync(token.SessionId.Value, cancellationToken);
        if (session is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.InvalidGrant,
                Description = "Invalid refresh token."
            });
        }

        if (token.Revoked)
        {
            var sessionResult = await _sessionCommandService.RemoveAsync(session, cancellationToken);
            if (!sessionResult.Succeeded) return sessionResult;

            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.InvalidGrant,
                Description = "Invalid refresh token."
            });
        }

        var client = await _clientQueryService.GetByIdAsync(flowContext.ClientId, cancellationToken);
        if (client is null)
        {
            return Results.ClientError(ClientErrorCode.Unauthorized, new Error
            {
                Code = ErrorCode.InvalidClient,
                Description = "Invalid client."
            });
        }

        var clientGrantTypes = await _clientQueryService.GetSupportedGrantTypesAsync(client.Id, cancellationToken);
        if (clientGrantTypes.All(x => x.Grant.Grant != flowContext.GrantType))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.UnsupportedGrantType,
                Description = $"'{flowContext.GrantType}' is not supported by client."
            });
        }

        if (client.Id != token.ClientId)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidGrant,
                Description = "Invalid refresh token"
            });
        }

        var clientScopes = await _clientQueryService.GetAllowedScopesAsync(client.Id, cancellationToken);
        if (clientScopes.All(x => x.Scope.Value != ScopeTypes.OfflineAccess))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidScope,
                Description = "offline_access scope was not originally granted."
            });
        }

        if (!client.AllowOfflineAccess)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidGrant,
                Description = "Refresh token grant is not allowed for this client."
            });
        }

        var user = await _userQueryService.GetBySubjectAsync(token.Subject, cancellationToken);
        if (user is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.InvalidGrant,
                Description = "Invalid refresh token."
            });
        }

        var response = new RefreshTokenResponse
        {
            ExpiresIn = (int)_tokenConfigurations.DefaultAccessTokenLifetime.TotalSeconds,
            TokenType = ResponseTokenType.Bearer,
        };

        var accessTokenFactoryContext = new AccessTokenFactoryContext
        {
            ClientId = client.Id,
            UserId = user.Id,
            SessionId = session.Id,
            TokenLifetime = client.AccessTokenLifetime,
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

        if (client.RefreshTokenRotationEnabled)
        {
            var refreshTokenFactoryContext = new RefreshTokenFactoryContext
            {
                ClientId = client.Id,
                UserId = user.Id,
                SessionId = session.Id,
                TokenLifetime = client.RefreshTokenLifetime
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
        else
        {
            response.RefreshToken = flowContext.RefreshToken;
        }

        var idTokenFactoryContext = new IdTokenFactoryContext
        {
            ClientId = client.Id,
            UserId = user.Id,
            SessionId = session.Id,
            TokenLifetime = client.IdTokenLifetime
        };
        
        var idTokenFactory = _tokenFactoryProvider.GetFactory<IdTokenFactoryContext>();
        var idTokenResult = await idTokenFactory.CreateAsync(idTokenFactoryContext, 
            cancellationToken: cancellationToken);
        
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