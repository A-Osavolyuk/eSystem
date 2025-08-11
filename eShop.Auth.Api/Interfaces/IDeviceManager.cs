using UAParser;

namespace eShop.Auth.Api.Interfaces;

public interface IDeviceManager
{
    public ValueTask<UserDeviceEntity?> FindAsync(UserEntity user, string userAgent, 
        string ipAddress, CancellationToken cancellationToken = default);
    public ValueTask<Result> TrustAsync(UserDeviceEntity device,  CancellationToken cancellationToken = default);
    public ValueTask<Result> DistrustAsync(UserDeviceEntity device,  CancellationToken cancellationToken = default);
    public ValueTask<Result> BlockAsync(UserDeviceEntity device,  CancellationToken cancellationToken = default);
    public ValueTask<Result> UnblockAsync(UserDeviceEntity device,  CancellationToken cancellationToken = default);
}