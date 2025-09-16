namespace eShop.Auth.Api.Interfaces;

public interface ITokenManager
{
    public Task<string> GenerateAsync(UserEntity user, 
        UserDeviceEntity device, CancellationToken cancellationToken = default);
    public Task<RefreshTokenEntity?> FindAsync(UserEntity user, 
        UserDeviceEntity device, CancellationToken cancellationToken = default);
}