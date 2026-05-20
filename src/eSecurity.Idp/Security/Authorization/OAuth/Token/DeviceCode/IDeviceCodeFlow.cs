using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authorization.OAuth.Token.DeviceCode;

public interface IDeviceCodeFlow
{
    public ValueTask<Result> ExecuteAsync(DeviceCodeEntity deviceCode, 
        DeviceCodeFlowContext context, CancellationToken cancellationToken = default);
}