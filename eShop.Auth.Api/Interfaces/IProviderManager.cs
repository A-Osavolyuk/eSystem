namespace eShop.Auth.Api.Interfaces;

public interface IProviderManager
{
    public ValueTask<List<ProviderEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    public ValueTask<ProviderEntity?> FindByNameAsync(string providerName, CancellationToken cancellationToken = default);
    public ValueTask<Result> SubscribeAsync(UserEntity user, ProviderEntity provider, CancellationToken cancellationToken = default);
    public ValueTask<Result> UnsubscribeAsync(UserEntity user, ProviderEntity provider, CancellationToken cancellationToken = default);
    public ValueTask<Result> UnsubscribeAsync(UserEntity user, CancellationToken cancellationToken = default);
}