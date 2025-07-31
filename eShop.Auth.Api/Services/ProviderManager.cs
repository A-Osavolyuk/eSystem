using eShop.Domain.Common.Security;

namespace eShop.Auth.Api.Services;

[Injectable(typeof(IProviderManager), ServiceLifetime.Scoped)]
public sealed class ProviderManager(AuthDbContext context) : IProviderManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<List<ProviderEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var providers = await context.Providers.ToListAsync(cancellationToken);
        return providers;
    }

    public async ValueTask<ProviderEntity?> FindByNameAsync(string providerName,
        CancellationToken cancellationToken = default)
    {
        var provider = await context.Providers.FirstOrDefaultAsync(x => x.Name == providerName, cancellationToken);
        return provider;
    }

    public async ValueTask<Result> SubscribeAsync(UserEntity user, ProviderEntity provider,
        CancellationToken cancellationToken = default)
    {
        var userProvider = await context.UserProvider
            .FirstOrDefaultAsync(x => x.UserId == user.Id && x.ProviderId == provider.Id, cancellationToken);

        if (userProvider is null)
        {
            return Results.NotFound("Not found user provider");
        }
        
        userProvider.Subscribed = true;
        userProvider.UpdateDate = DateTimeOffset.UtcNow;

        context.UserProvider.Update(userProvider);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> UnsubscribeAsync(UserEntity user, ProviderEntity provider,
        CancellationToken cancellationToken = default)
    {
        var userProvider = await context.UserProvider
            .FirstOrDefaultAsync(x => x.UserId == user.Id && x.ProviderId == provider.Id, cancellationToken);

        if (userProvider is null)
        {
            return Results.NotFound("Not found user provider");
        }

        userProvider.Subscribed = false;
        userProvider.UpdateDate = DateTimeOffset.UtcNow;
        
        context.UserProvider.Update(userProvider);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}