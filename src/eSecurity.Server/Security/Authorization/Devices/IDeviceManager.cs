using eSecurity.Server.Data.Entities;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authorization.Devices;

public interface IDeviceManager
{
    public ValueTask<List<UserDeviceEntity>> GetAllAsync(UserEntity user, 
        CancellationToken cancellationToken = default);
    
    public ValueTask<UserDeviceEntity?> FindAsync(UserEntity user, string userAgent, string ipAddress,
        CancellationToken cancellationToken);

    public ValueTask<UserDeviceEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public ValueTask<Result> CreateAsync(UserDeviceEntity device, CancellationToken cancellationToken = default);
    public ValueTask<Result> BlockAsync(UserDeviceEntity device, CancellationToken cancellationToken = default);
    public ValueTask<Result> UnblockAsync(UserDeviceEntity device, CancellationToken cancellationToken = default);
}