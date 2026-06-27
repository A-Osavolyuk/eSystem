using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authorization.Devices;

public interface IDeviceCommandService
{
    ValueTask<Result> CreateAsync(UserDeviceEntity entity, CancellationToken cancellationToken = default);

    ValueTask<Result> BlockAsync(Guid deviceId, CancellationToken cancellationToken = default);
}