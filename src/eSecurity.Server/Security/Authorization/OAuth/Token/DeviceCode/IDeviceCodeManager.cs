using eSecurity.Server.Data.Entities;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.DeviceCode;

public interface IDeviceCodeManager
{
    public ValueTask<DeviceCodeEntity?> FindByHashAsync(string deviceCodeHash,
        CancellationToken cancellationToken = default);

    public ValueTask<DeviceCodeEntity?> FindByCodeAsync(string userCode, 
        CancellationToken cancellationToken = default);
    
    public ValueTask<Result> CreateAsync(DeviceCodeEntity deviceCode, 
        CancellationToken cancellationToken = default);
    
    public ValueTask<Result> UpdateAsync(DeviceCodeEntity deviceCode, 
        CancellationToken cancellationToken = default);
}