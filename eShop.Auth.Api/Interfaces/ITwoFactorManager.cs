namespace eShop.Auth.Api.Interfaces;

public interface ITwoFactorManager
{
    public ValueTask<List<ProviderEntity>> GetProvidersAsync(CancellationToken cancellationToken = default);
    public ValueTask<Result> EnableAsync(UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<Result> DisableAsync(UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<string> GenerateTokenAsync(UserEntity user, Provider provider, CancellationToken cancellationToken = default);
    public ValueTask<string> GenerateQrAsync(UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<bool> VerifyTokenAsync(UserEntity user, Provider provider, string token, CancellationToken cancellationToken = default);
}