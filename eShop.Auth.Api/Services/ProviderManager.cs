using eShop.Domain.Common.Security;

namespace eShop.Auth.Api.Services;

[Injectable(typeof(IProviderManager), ServiceLifetime.Scoped)]
public sealed class ProviderManager(
    AuthDbContext context,
    ISecretManager secretManager) : IProviderManager
{
    private readonly AuthDbContext context = context;
    private readonly ISecretManager secretManager = secretManager;

    public async ValueTask<List<ProviderEntity>> GetProvidersAsync(CancellationToken cancellationToken = default)
    {
        var providers = await context.Providers.ToListAsync(cancellationToken);
        return providers;
    }

    public async ValueTask<List<UserProviderEntity>> GetProvidersAsync(UserEntity user,
        CancellationToken cancellationToken = default)
    {
        var providers = await context.UserProvider
            .Where(x => x.UserId == user.Id)
            .Include(x => x.Provider)
            .ToListAsync(cancellationToken);

        return providers;
    }

    public async ValueTask<ProviderEntity?> FindAsync(string providerName,
        CancellationToken cancellationToken = default)
    {
        var provider = await context.Providers.FirstOrDefaultAsync(x => x.Name == providerName, cancellationToken);
        return provider;
    }

    public async ValueTask<Result> SubscribeAsync(UserEntity user, ProviderEntity provider,
        CancellationToken cancellationToken = default)
    {
        var userProvider = new UserProviderEntity()
        {
            UserId = user.Id,
            ProviderId = provider.Id,
            Subscribed = true,
            CreateDate = DateTime.UtcNow,
            UpdateDate = null
        };

        if (provider.Name == ProviderTypes.Authenticator)
        {
            await secretManager.GenerateAsync(user, cancellationToken);
        }

        await context.UserProvider.AddAsync(userProvider, cancellationToken);
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
        
        context.UserProvider.Update(userProvider);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}