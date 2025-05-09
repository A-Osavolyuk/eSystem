namespace eShop.Auth.Api.Interfaces;

public interface ITwoFactorManager
{
    public ValueTask<Result> EnableAsync(UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<Result> DisableAsync(UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<QrCode> GenerateQrCodeAsync(UserEntity user, string secret, CancellationToken cancellationToken = default);
}