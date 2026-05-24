using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.Client;
using eSecurity.Idp.Security.Cryptography.Tokens;
using eSecurity.Idp.Security.Cryptography.Tokens.Access;
using eSecurity.Idp.Security.Cryptography.Tokens.Login;
using eSecurity.Idp.Security.Cryptography.Tokens.Refresh;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Server.Security.Authorization.OAuth.Token.DeviceCode;

namespace eSecurity.Idp.Security.Authorization.Token.DeviceCode;

public sealed class OAuthDeviceCodeFlow(
    IClientManager clientManager,
    IDeviceCodeManager deviceCodeManager,
    IUserManager userManager,
    ITokenFactoryProvider tokenFactoryProvider,
    IOptions<TokenConfigurations> tokenOptions) : IDeviceCodeFlow
{
    private readonly IClientManager _clientManager = clientManager;
    private readonly IDeviceCodeManager _deviceCodeManager = deviceCodeManager;
    private readonly IUserManager _userManager = userManager;
    private readonly ITokenFactoryProvider _tokenFactoryProvider = tokenFactoryProvider;
    private readonly TokenConfigurations _tokenConfigurations = tokenOptions.Value;

    public async ValueTask<Result> ExecuteAsync(DeviceCodeEntity deviceCode, DeviceCodeFlowContext context,
        CancellationToken cancellationToken = default)
    {
        var client = await _clientManager.FindByIdAsync(context.ClientId, cancellationToken);
        if (client is null || deviceCode.UserId is null)
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

        var user = await _userManager.FindByIdAsync(deviceCode.UserId.Value, cancellationToken);
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
        
        if (client.AllowOfflineAccess && client.HasScope(ScopeTypes.OfflineAccess))
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

        if (client.HasGrantType(GrantType.Ciba))
        {
            var loginTokenFactoryContext = new LoginTokenFactoryContext
            {
                Client = client,
                User = user
            };
                
            var loginTokenFactory = _tokenFactoryProvider.GetFactory<LoginTokenFactoryContext>();
            var loginTokenResult = await loginTokenFactory.CreateAsync(loginTokenFactoryContext, 
                cancellationToken: cancellationToken);
            
            if (!loginTokenResult.Succeeded)
            {
                var error = loginTokenResult.GetError();
                return Results.ServerError(ServerErrorCode.InternalServerError, error);
            }

            if (!loginTokenResult.TryGetValue(out var loginToken))
            {
                return Results.ServerError(ServerErrorCode.InternalServerError, new Error
                {
                    Code = ErrorCode.ServerError,
                    Description = "Server error"
                });
            }
            
            response.LoginTokenHint = loginToken;
        }
        
        return Results.Success(SuccessCodes.Ok, response);
    }
}