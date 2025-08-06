namespace eShop.Auth.Api.Services;

[Injectable(typeof(IOAuthProviderManager), ServiceLifetime.Scoped)]
public class OAuthProviderManager(AuthDbContext context) : IOAuthProviderManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<List<OAuthProviderEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await context.OAuthProviders.ToListAsync(cancellationToken);
        return entities;
    }

    public async ValueTask<OAuthProviderEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await context.OAuthProviders.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return entity;
    }

    public async ValueTask<OAuthProviderEntity?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var entity = await context.OAuthProviders.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
        return entity;
    }
}