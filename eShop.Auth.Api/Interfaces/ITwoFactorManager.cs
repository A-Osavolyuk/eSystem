namespace eShop.Auth.Api.Interfaces;

public interface ITwoFactorManager
{
    public ValueTask<List<ProviderEntity>> GetProvidersAsync(CancellationToken cancellationToken = default);
    public ValueTask<List<ProviderEntity>> GetProvidersAsync(UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<ProviderEntity?> GetProviderAsync(string providerName, CancellationToken cancellationToken = default);
    public ValueTask<Result> EnableAsync(UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<Result> DisableAsync(UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<string> GenerateTokenAsync(UserEntity user, ProviderEntity provider, CancellationToken cancellationToken = default);
    public ValueTask<QrCode> GenerateQrCodeAsync(UserEntity user, string secret, CancellationToken cancellationToken = default);
    public ValueTask<Result> VerifyTokenAsync(UserEntity user, string token, CancellationToken cancellationToken = default);
    public ValueTask<Result> SubscribeAsync(UserEntity user, ProviderEntity provider, CancellationToken cancellationToken = default);
    public ValueTask<Result> UnsubscribeAsync(UserEntity user, ProviderEntity provider, CancellationToken cancellationToken = default);
}