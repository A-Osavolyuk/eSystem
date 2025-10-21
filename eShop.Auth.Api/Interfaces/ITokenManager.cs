namespace eShop.Auth.Api.Interfaces;

public interface ITokenManager
{
    public Task<string> CreateAsync(UserDeviceEntity device, CancellationToken cancellationToken = default);
    public Task<RefreshTokenEntity?> FindAsync(UserDeviceEntity device, CancellationToken cancellationToken = default);
    public Task<Result> RemoveAsync(RefreshTokenEntity token, CancellationToken cancellationToken = default);
}