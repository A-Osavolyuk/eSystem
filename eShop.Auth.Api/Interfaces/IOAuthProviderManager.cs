namespace eShop.Auth.Api.Interfaces;

public interface IOAuthProviderManager
{
    public ValueTask<List<OAuthProviderEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    public ValueTask<OAuthProviderEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public ValueTask<OAuthProviderEntity?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
}