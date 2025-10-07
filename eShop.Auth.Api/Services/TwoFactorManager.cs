namespace eShop.Auth.Api.Services;

[Injectable(typeof(ITwoFactorManager), ServiceLifetime.Scoped)]
public sealed class TwoFactorManager(AuthDbContext context) : ITwoFactorManager
{
    private readonly AuthDbContext context = context;
    
    public async ValueTask<Result> SubscribeAsync(UserEntity user, TwoFactorMethod method, bool isPrimary = false,
        CancellationToken cancellationToken = default)
    {
        var userProvider = new UserTwoFactorMethodEntity()
        {
            UserId = user.Id,
            Method = method,
            IsPrimary = isPrimary,
            CreateDate = DateTimeOffset.UtcNow
        };

        if (!user.TwoFactorEnabled)
        {
            user.TwoFactorEnabled = true;
            user.UpdateDate = DateTimeOffset.UtcNow;
        }

        context.Users.Update(user);
        await context.UserTwoFactorMethods.AddAsync(userProvider, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> UnsubscribeAsync(UserEntity user, 
        CancellationToken cancellationToken = default)
    {
        user.TwoFactorEnabled = false;
        user.UpdateDate = DateTimeOffset.UtcNow;
        
        context.Users.Update(user);
        context.UserTwoFactorMethods.RemoveRange(user.Methods);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}