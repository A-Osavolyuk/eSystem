namespace eShop.Auth.Api.Services;

[Injectable(typeof(ITwoFactorProviderManager), ServiceLifetime.Scoped)]
public sealed class TwoFactorProviderManager(AuthDbContext context) : ITwoFactorProviderManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<List<TwoFactorProviderEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var providers = await context.TwoFactorProviders.ToListAsync(cancellationToken);
        return providers;
    }

    public async ValueTask<TwoFactorProviderEntity?> FindByNameAsync(string providerName,
        CancellationToken cancellationToken = default)
    {
        var provider = await context.TwoFactorProviders.FirstOrDefaultAsync(x => x.Name == providerName, cancellationToken);
        return provider;
    }

    public async ValueTask<TwoFactorProviderEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var provider = await context.TwoFactorProviders.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return provider;
    }

    public async ValueTask<Result> SubscribeAsync(UserEntity user, TwoFactorProviderEntity twoFactorProvider,
        CancellationToken cancellationToken = default)
    {
        var userProvider = await context.UserTwoFactorProviders
            .FirstOrDefaultAsync(x => x.UserId == user.Id && x.ProviderId == twoFactorProvider.Id, cancellationToken);

        if (userProvider is null)
        {
            return Results.NotFound("Not found user provider");
        }
        
        userProvider.Subscribed = true;
        userProvider.UpdateDate = DateTimeOffset.UtcNow;

        context.UserTwoFactorProviders.Update(userProvider);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> UnsubscribeAsync(UserEntity user, TwoFactorProviderEntity twoFactorProvider,
        CancellationToken cancellationToken = default)
    {
        var userProvider = await context.UserTwoFactorProviders
            .FirstOrDefaultAsync(x => x.UserId == user.Id && x.ProviderId == twoFactorProvider.Id, cancellationToken);

        if (userProvider is null)
        {
            return Results.NotFound("Not found user provider");
        }

        userProvider.Subscribed = false;
        userProvider.UpdateDate = DateTimeOffset.UtcNow;
        
        context.UserTwoFactorProviders.Update(userProvider);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}