namespace eShop.Auth.Api.Interfaces;

public interface IOAuthProviderManager
{
    public ValueTask<OAuthProviderEntity?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
}