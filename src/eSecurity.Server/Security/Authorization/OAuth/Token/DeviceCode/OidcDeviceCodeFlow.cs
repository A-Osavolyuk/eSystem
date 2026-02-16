using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Constants;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Server.Security.Cryptography.Tokens;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;
using eSystem.Core.Security.Authorization.OAuth.Constants;
using eSystem.Core.Security.Authorization.OAuth.Token.DeviceCode;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.DeviceCode;

public sealed class OidcDeviceCodeFlow(
    IClientManager clientManager,
    IDeviceCodeManager deviceCodeManager,
    IUserManager userManager,
    ISessionManager sessionManager,
    ITokenFactoryProvider tokenFactoryProvider,
    IOptions<TokenConfigurations> tokenOptions) : IDeviceCodeFlow
{
    private readonly IClientManager _clientManager = clientManager;
    private readonly IDeviceCodeManager _deviceCodeManager = deviceCodeManager;
    private readonly IUserManager _userManager = userManager;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly ITokenFactoryProvider _tokenFactoryProvider = tokenFactoryProvider;
    private readonly TokenConfigurations _tokenConfigurations = tokenOptions.Value;

    public async ValueTask<Result> ExecuteAsync(DeviceCodeEntity deviceCode, DeviceCodeFlowContext context,
        CancellationToken cancellationToken = default)
    {
        var client = await _clientManager.FindByIdAsync(context.ClientId, cancellationToken);
        if (client is null || deviceCode.UserId is null || deviceCode.SessionId is null)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Invalid device code"
            });
        }

        deviceCode.State = DeviceCodeState.Consumed;
        deviceCode.ConsumedAt = DateTimeOffset.UtcNow;
        deviceCode.IsFirstPoll = false;

        var deviceResult = await _deviceCodeManager.UpdateAsync(deviceCode, cancellationToken);
        if (deviceResult.Succeeded) return deviceResult;

        var user = await _userManager.FindByIdAsync(deviceCode.UserId.Value, cancellationToken);
        if (user is null)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Invalid device code"
            });
        }

        var response = new DeviceCodeResponse
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

        var clientResult = await _clientManager.RelateAsync(client, session, cancellationToken);
        if (!clientResult.Succeeded) return clientResult;

        if (client.AllowOfflineAccess && client.HasScope(ScopeTypes.OfflineAccess))
        {
            var refreshTokenFactory = _tokenFactoryProvider.GetFactory(TokenType.RefreshToken);
            var refreshTokenResult = await refreshTokenFactory.CreateAsync(client, user, 
                session, cancellationToken: cancellationToken);
            
            if (!refreshTokenResult.Succeeded)
            {
                var error = refreshTokenResult.GetError();
                return Results.InternalServerError(error);
            }

            if (!accessTokenResult.TryGetValue(out var refreshToken))
            {
                return Results.InternalServerError(new Error()
                {
                    Code = ErrorTypes.OAuth.ServerError,
                    Description = "Server error"
                });
            }
            
            response.RefreshToken = refreshToken;
        }
        
        if (client.HasGrantType(GrantTypes.Ciba))
        {
            var loginTokenFactory = _tokenFactoryProvider.GetFactory(TokenType.LoginToken);
            var loginTokenResult = await loginTokenFactory.CreateAsync(client, user, 
                session, cancellationToken: cancellationToken);
            
            if (!loginTokenResult.Succeeded)
            {
                var error = loginTokenResult.GetError();
                return Results.InternalServerError(error);
            }

            if (!accessTokenResult.TryGetValue(out var loginToken))
            {
                return Results.InternalServerError(new Error()
                {
                    Code = ErrorTypes.OAuth.ServerError,
                    Description = "Server error"
                });
            }
            
            response.LoginTokenHint = loginToken;
        }

        var idTokenFactory = _tokenFactoryProvider.GetFactory(TokenType.IdToken);
        var idTokenResult = await idTokenFactory.CreateAsync(client, user, 
            session, cancellationToken: cancellationToken);
        
        if (!idTokenResult.Succeeded)
        {
            var error = idTokenResult.GetError();
            return Results.InternalServerError(error);
        }

        if (!idTokenResult.TryGetValue(out var idToken))
        {
            return Results.InternalServerError(new Error()
            {
                Code = ErrorTypes.OAuth.ServerError,
                Description = "Server error"
            });
        }
            
        response.IdToken = idToken;
        
        return Results.Ok(response);
    }
}