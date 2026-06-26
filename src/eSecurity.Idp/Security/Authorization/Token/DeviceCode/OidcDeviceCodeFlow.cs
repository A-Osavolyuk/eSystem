using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.Client;
using eSecurity.Idp.Security.Authentication.Session;
using eSecurity.Idp.Security.Cryptography.Tokens;
using eSecurity.Idp.Security.Cryptography.Tokens.Access;
using eSecurity.Idp.Security.Cryptography.Tokens.Id;
using eSecurity.Idp.Security.Cryptography.Tokens.Login;
using eSecurity.Idp.Security.Cryptography.Tokens.Refresh;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Server.Security.Authorization.OAuth.Token.DeviceCode;

namespace eSecurity.Idp.Security.Authorization.Token.DeviceCode;

public sealed class OidcDeviceCodeFlow(
    IDeviceCodeManager deviceCodeManager,
    IUserQueryService userQueryService,
    ITokenFactoryProvider tokenFactoryProvider,
    IClientQueryService clientQueryService,
    IClientCommandService clientCommandService,
    IOptions<TokenConfigurations> tokenOptions,
    ISessionQueryService sessionQueryService,
    ISessionAccessor sessionAccessor) : IDeviceCodeFlow
{
    private readonly IDeviceCodeManager _deviceCodeManager = deviceCodeManager;
    private readonly IUserQueryService _userQueryService = userQueryService;
    private readonly ITokenFactoryProvider _tokenFactoryProvider = tokenFactoryProvider;
    private readonly IClientQueryService _clientQueryService = clientQueryService;
    private readonly IClientCommandService _clientCommandService = clientCommandService;
    private readonly ISessionQueryService _sessionQueryService = sessionQueryService;
    private readonly ISessionAccessor _sessionAccessor = sessionAccessor;
    private readonly TokenConfigurations _tokenConfigurations = tokenOptions.Value;

    public async ValueTask<Result> ExecuteAsync(DeviceCodeEntity deviceCode, DeviceCodeFlowContext context,
        CancellationToken cancellationToken = default)
    {
        var client = await _clientQueryService.GetByIdAsync(context.ClientId, cancellationToken);
        if (client is null || deviceCode.UserId is null || deviceCode.SessionId is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidGrant,
                Description = "Invalid device code"
            });
        }

        deviceCode.State = DeviceCodeState.Consumed;
        deviceCode.ConsumedAt = DateTimeOffset.UtcNow;
        deviceCode.IsFirstPoll = false;

        var deviceResult = await _deviceCodeManager.UpdateAsync(deviceCode, cancellationToken);
        if (deviceResult.Succeeded) return deviceResult;

        var user = await _userQueryService.GetByIdAsync(deviceCode.UserId.Value, cancellationToken);
        if (user is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidGrant,
                Description = "Invalid device code"
            });
        }

        var response = new DeviceCodeResponse
        {
            ExpiresIn = (int)_tokenConfigurations.DefaultAccessTokenLifetime.TotalSeconds,
            TokenType = ResponseTokenType.Bearer,
        };

        var sessionCookie = _sessionAccessor.GetCookie();
        if (sessionCookie is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.UnauthorizedClient,
                Description = "Unauthorized client."
            });
        }
        
        var session = await _sessionQueryService.GetByIdAsync(sessionCookie.SessionId, cancellationToken);
        if (session is null || session.ExpireDate < DateTimeOffset.UtcNow)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidGrant,
                Description = "Invalid authorization code."
            });
        }

        var accessTokenFactoryContext = new AccessTokenFactoryContext
        {
            Client = client,
            User = user,
            Session = session
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

        var clientScopes = await _clientQueryService.GetAllowedScopesAsync(client, cancellationToken);
        if (client.AllowOfflineAccess && clientScopes.Any(x => x.Scope.Value == ScopeTypes.OfflineAccess))
        {
            var refreshTokenFactoryContext = new RefreshTokenFactoryContext
            {
                Client = client,
                User = user,
                Session = session
            };
            
            var refreshTokenFactory = _tokenFactoryProvider.GetFactory<RefreshTokenFactoryContext>();
            var refreshTokenResult = await refreshTokenFactory.CreateAsync(refreshTokenFactoryContext, 
                cancellationToken: cancellationToken);
            
            if (!refreshTokenResult.Succeeded)
            {
                var error = refreshTokenResult.GetError();
                return Results.ServerError(ServerErrorCode.InternalServerError, error);
            }

            if (!accessTokenResult.TryGetValue(out var refreshToken))
            {
                return Results.ServerError(ServerErrorCode.InternalServerError, new Error
                {
                    Code = ErrorCode.ServerError,
                    Description = "Server error"
                });
            }
            
            response.RefreshToken = refreshToken;
        }

        var clientGrantTypes = await _clientQueryService.GetSupportedGrantTypesAsync(client, cancellationToken);
        if (clientGrantTypes.Any(x => x.Grant.Grant == GrantType.Ciba))
        {
            var loginTokenFactoryContext = new LoginTokenFactoryContext
            {
                Client = client,
                User = user,
                Session = session
            };
            
            var loginTokenFactory = _tokenFactoryProvider.GetFactory<LoginTokenFactoryContext>();
            var loginTokenResult = await loginTokenFactory.CreateAsync(loginTokenFactoryContext, 
                cancellationToken: cancellationToken);
            
            if (!loginTokenResult.Succeeded)
            {
                var error = loginTokenResult.GetError();
                return Results.ServerError(ServerErrorCode.InternalServerError, error);
            }

            if (!accessTokenResult.TryGetValue(out var loginToken))
            {
                return Results.ServerError(ServerErrorCode.InternalServerError, new Error
                {
                    Code = ErrorCode.ServerError,
                    Description = "Server error"
                });
            }
            
            response.LoginTokenHint = loginToken;
        }

        var idTokenFactoryContext = new IdTokenFactoryContext
        {
            Client = client,
            User = user,
            Session = session
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