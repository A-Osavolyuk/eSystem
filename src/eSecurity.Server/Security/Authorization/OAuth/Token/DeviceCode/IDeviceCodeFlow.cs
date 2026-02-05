using eSecurity.Server.Data.Entities;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.DeviceCode;

public interface IDeviceCodeFlow
{
    public ValueTask<Result> ExecuteAsync(DeviceCodeEntity deviceCode, 
        DeviceCodeFlowContext context, CancellationToken cancellationToken = default);
}