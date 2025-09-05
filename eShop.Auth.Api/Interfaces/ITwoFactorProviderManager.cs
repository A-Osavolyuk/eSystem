namespace eShop.Auth.Api.Interfaces;

public interface ITwoFactorProviderManager
{
    public ValueTask<List<TwoFactorProviderEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    public ValueTask<TwoFactorProviderEntity?> FindByNameAsync(string providerName, CancellationToken cancellationToken = default);
    public ValueTask<TwoFactorProviderEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public ValueTask<Result> SubscribeAsync(UserEntity user, TwoFactorProviderEntity twoFactorProvider, CancellationToken cancellationToken = default);
    public ValueTask<Result> UnsubscribeAsync(UserEntity user, TwoFactorProviderEntity twoFactorProvider, CancellationToken cancellationToken = default);
}