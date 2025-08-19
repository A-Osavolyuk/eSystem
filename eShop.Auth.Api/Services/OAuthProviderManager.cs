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

    public async ValueTask<Result> ConnectAsync(UserEntity user, OAuthProviderEntity provider, CancellationToken cancellationToken = default)
    {
        var existedEntity = await context.UserOAuthProviders.FirstOrDefaultAsync(
            x => x.UserId == user.Id && x.ProviderId == provider.Id, cancellationToken);

        if (existedEntity is not null)
        {
            return Result.Success();
        }

        var entity = new UserOAuthProviderEntity()
        {
            UserId = user.Id,
            ProviderId = provider.Id,
            Allowed = true,
            CreateDate = DateTimeOffset.UtcNow
        };
        
        await context.UserOAuthProviders.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> DisconnectAsync(UserEntity user, 
        OAuthProviderEntity provider, CancellationToken cancellationToken = default)
    {
        var entity = await context.UserOAuthProviders.FirstOrDefaultAsync(
            x => x.UserId == user.Id && x.ProviderId == provider.Id, cancellationToken);

        if (entity is null)
        {
            return Results.NotFound("Cannot disconnect. Provider was not connected.");
        }
        
        context.UserOAuthProviders.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}