namespace eShop.Auth.Api.Services;

[Injectable(typeof(ITwoFactorManager), ServiceLifetime.Scoped)]
public sealed class TwoFactorManager(AuthDbContext context) : ITwoFactorManager
{
    private readonly AuthDbContext context = context;
    
    public async ValueTask<TwoFactorMethodEntity?> FindByTypeAsync(MethodType type,
        CancellationToken cancellationToken = default)
    {
        var provider = await context.TwoFactorProviders.FirstOrDefaultAsync(
            x => x.Type == type, cancellationToken);
        
        return provider;
    }

    public async ValueTask<Result> SubscribeAsync(UserEntity user, MethodType type, bool isPrimary = false,
        CancellationToken cancellationToken = default)
    {
        var provider = await FindByTypeAsync(type, cancellationToken);
        if (provider is null) return Results.NotFound($"Cannot find 2FA provider with type {type}.");

        var userProvider = new UserTwoFactorMethodEntity()
        {
            UserId = user.Id,
            ProviderId = provider.Id,
            IsPrimary = isPrimary,
            CreateDate = DateTimeOffset.UtcNow
        };

        if (!user.TwoFactorEnabled)
        {
            user.TwoFactorEnabled = true;
            user.UpdateDate = DateTimeOffset.UtcNow;
        }

        context.Users.Update(user);
        await context.UserTwoFactorProviders.AddAsync(userProvider, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> UnsubscribeAsync(UserEntity user, UserTwoFactorMethodEntity method, 
        CancellationToken cancellationToken = default)
    {
        user.TwoFactorEnabled = false;
        user.UpdateDate = DateTimeOffset.UtcNow;
        
        context.Users.Update(user);
        context.UserTwoFactorProviders.Remove(method);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}