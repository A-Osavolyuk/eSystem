namespace eShop.Auth.Api.Interfaces;

public interface ITwoFactorManager
{
    public ValueTask<TwoFactorProviderEntity?> FindByTypeAsync(ProviderType type, 
        CancellationToken cancellationToken = default);
    public ValueTask<Result> SubscribeAsync(UserEntity user, ProviderType type, bool isPrimary = false,
        CancellationToken cancellationToken = default);
    public ValueTask<Result> UnsubscribeAsync(UserEntity user, CancellationToken cancellationToken = default);
}