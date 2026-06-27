using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Authorization.Devices;

public interface IDeviceQueryService
{
    ValueTask<List<UserDeviceEntity>> ListByUserAsync(Guid userId, CancellationToken cancellationToken = default);

    ValueTask<UserDeviceEntity?> GetByMetadataAsync(Guid userId, string userAgent, string ipAddress,
        CancellationToken cancellationToken = default);

    ValueTask<UserDeviceEntity?> GetByIdAsync(Guid deviceId, CancellationToken cancellationToken = default);
}