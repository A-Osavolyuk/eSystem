using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Cryptography.Tokens;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Security.Authorization.OAuth.Token.DeviceCode;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.DeviceCode;

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
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
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
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
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

        var accessTokenFactory = _tokenFactoryProvider.GetFactory(TokenType.AccessToken);
        var accessTokenResult = await accessTokenFactory.CreateAsync(client, user, cancellationToken: cancellationToken);
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
        
        if (client.AllowOfflineAccess && client.HasScope(ScopeTypes.OfflineAccess))
        {
            var refreshTokenFactory = _tokenFactoryProvider.GetFactory(TokenType.RefreshToken);
            var refreshTokenResult = await refreshTokenFactory.CreateAsync(client, user, cancellationToken: cancellationToken);
            if (!refreshTokenResult.Succeeded)
            {
                var error = refreshTokenResult.GetError();
                return Results.ServerError(ServerErrorCode.InternalServerError, error);
            }

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

        if (client.HasGrantType(GrantType.Ciba))
        {
            var loginTokenFactory = _tokenFactoryProvider.GetFactory(TokenType.LoginToken);
            var loginTokenResult = await loginTokenFactory.CreateAsync(client, user, cancellationToken: cancellationToken);
            if (!loginTokenResult.Succeeded)
            {
                var error = loginTokenResult.GetError();
                return Results.ServerError(ServerErrorCode.InternalServerError, error);
            }

            if (!loginTokenResult.TryGetValue(out var loginToken))
            {
                return Results.ServerError(ServerErrorCode.InternalServerError, new Error()
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