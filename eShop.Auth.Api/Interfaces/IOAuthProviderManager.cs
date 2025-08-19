namespace eShop.Auth.Api.Interfaces;

public interface IOAuthProviderManager
{
    public ValueTask<List<OAuthProviderEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    public ValueTask<OAuthProviderEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public ValueTask<OAuthProviderEntity?> FindByNameAsync(string name, CancellationToken cancellationToken = default);

    public ValueTask<Result> ConnectAsync(UserEntity user, OAuthProviderEntity provider,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> DisconnectAsync(UserEntity user, OAuthProviderEntity provider,
        CancellationToken cancellationToken = default);
    
    public ValueTask<Result> AllowAsync(UserEntity user, OAuthProviderEntity provider,
        CancellationToken cancellationToken = default);
    
    public ValueTask<Result> DisallowAsync(UserEntity user, OAuthProviderEntity provider,
        CancellationToken cancellationToken = default);
}