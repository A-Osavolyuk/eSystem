namespace eShop.Auth.Api.Interfaces;

public interface ITwoFactorManager
{
    public ValueTask<List<TwoFactorProviderEntity>> GetAllAsync(
        CancellationToken cancellationToken = default);
    public ValueTask<TwoFactorProviderEntity?> FindByTypeAsync(ProviderType type, 
        CancellationToken cancellationToken = default);
    public ValueTask<TwoFactorProviderEntity?> FindByIdAsync(Guid id, 
        CancellationToken cancellationToken = default);
    public ValueTask<Result> SubscribeAsync(UserEntity user, TwoFactorProviderEntity twoFactorProvider, 
        CancellationToken cancellationToken = default);
    public ValueTask<Result> UnsubscribeAsync(List<UserTwoFactorProviderEntity> providers,
        CancellationToken cancellationToken = default);
}