using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.DeviceCode;

public interface IDeviceCodeFlow
{
    public ValueTask<Result> ExecuteAsync(DeviceCodeEntity deviceCode, 
        DeviceCodeFlowContext context, CancellationToken cancellationToken = default);
}