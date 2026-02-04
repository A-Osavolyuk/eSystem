using eSecurity.Server.Security.Authorization.Protocol;
using eSecurity.Server.Security.Authorization.Token.DeviceCode;
using eSecurity.Server.Security.Cryptography.Hashing;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;

namespace eSecurity.Server.Security.Authorization.Token.Strategies;

public sealed class DeviceCodeContext : TokenContext
{
    public string? DeviceCode { get; set; }
}

public sealed class DeviceCodeStrategy(
    IDeviceCodeManager deviceCodeManager,
    IHasherProvider hasherProvider,
    IDeviceCodeFlowResolver resolver) : ITokenStrategy
{
    private readonly IDeviceCodeManager _deviceCodeManager = deviceCodeManager;
    private readonly IDeviceCodeFlowResolver _resolver = resolver;
    private readonly IHasher _hasher = hasherProvider.GetHasher(HashAlgorithm.Sha512);

    public async ValueTask<Result> ExecuteAsync(TokenContext context, CancellationToken cancellationToken = default)
    {
        if (context is not DeviceCodeContext deviceContext)
            throw new NotSupportedException("Payload type must be 'DeviceCodeContext'.");

        if (string.IsNullOrEmpty(deviceContext.DeviceCode))
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Invalid device code"
            });
        }
        
        var deviceCodeHash = _hasher.Hash(deviceContext.DeviceCode);
        var deviceCode = await _deviceCodeManager.FindByHashAsync(deviceCodeHash, cancellationToken);
        if (deviceCode is null || deviceCode.State == DeviceCodeState.Consumed || 
            deviceCode is { State: DeviceCodeState.Denied, IsFirstPoll: false } ||
            deviceCode.ClientId.ToString() != deviceContext.ClientId)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Invalid device code"
            });
        }

        if (deviceCode is { State: DeviceCodeState.Denied, IsFirstPoll: true })
        {
            deviceCode.IsFirstPoll = false;
            
            var deviceResult = await _deviceCodeManager.UpdateAsync(deviceCode, cancellationToken);
            if (!deviceResult.Succeeded) return deviceResult;
                
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.AccessDenied,
                Description = "Device code was denied"
            });
        }

        if (deviceCode.State == DeviceCodeState.Pending)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.AuthorizationPending,
                Description = "Authorization pending"
            });
        }

        if (deviceCode.ExpiresAt < DateTimeOffset.UtcNow)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.ExpiredToken,
                Description = "Device code is already expired"
            });
        }

        var scopes = deviceCode.Scope.Split(' ').ToList();
        var protocol = scopes.Contains(ScopeTypes.OpenId) 
            ? AuthorizationProtocol.OpenIdConnect 
            : AuthorizationProtocol.OAuth;
        
        var deviceCodeFlowContext = new DeviceCodeFlowContext() { ClientId = deviceContext.ClientId };
        var flow = _resolver.Resolve(protocol);
        
        return await flow.ExecuteAsync(deviceCode, deviceCodeFlowContext, cancellationToken);
    }
}