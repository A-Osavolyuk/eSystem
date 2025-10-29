using eSystem.Auth.Api.Data.Entities;

namespace eSystem.Auth.Api.Interfaces;

public interface IDeviceManager
{
    public ValueTask<UserDeviceEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public ValueTask<Result> CreateAsync(UserDeviceEntity device,  CancellationToken cancellationToken = default);
    public ValueTask<Result> TrustAsync(UserDeviceEntity device,  CancellationToken cancellationToken = default);
    public ValueTask<Result> BlockAsync(UserDeviceEntity device,  CancellationToken cancellationToken = default);
    public ValueTask<Result> UnblockAsync(UserDeviceEntity device,  CancellationToken cancellationToken = default);
}