namespace eShop.Auth.Api.Interfaces;

public interface ITwoFactorManager
{
    public ValueTask<string> GenerateQrCodeAsync(UserEntity user, CancellationToken cancellationToken = default);
}