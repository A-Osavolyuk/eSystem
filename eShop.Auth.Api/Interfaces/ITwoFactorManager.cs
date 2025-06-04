namespace eShop.Auth.Api.Interfaces;

public interface ITwoFactorManager
{
    public ValueTask<Result> EnableAsync(UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<Result> DisableAsync(UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<string> GenerateQrCodeAsync(UserEntity user, CancellationToken cancellationToken = default);
}