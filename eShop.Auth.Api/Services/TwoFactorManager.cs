namespace eShop.Auth.Api.Services;

[Injectable(typeof(ITwoFactorManager), ServiceLifetime.Scoped)]
public sealed class TwoFactorManager(AuthDbContext context) : ITwoFactorManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<Result> SubscribeAsync(UserEntity user, TwoFactorMethod method,
        bool preferred = false, CancellationToken cancellationToken = default)
    {
        if (preferred && user.Methods.Any(x => x.Preferred))
        {
            var preferredMethod = user.Methods.First(x => x.Preferred);
            preferredMethod.Preferred = false;
            preferredMethod.UpdateDate = DateTimeOffset.UtcNow;

            context.UserTwoFactorMethods.Update(preferredMethod);
        }

        var userProvider = new UserTwoFactorMethodEntity()
        {
            UserId = user.Id,
            Method = method,
            Preferred = preferred,
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

    public async ValueTask<Result> UnsubscribeAsync(UserTwoFactorMethodEntity method,
        CancellationToken cancellationToken = default)
    {
        context.UserTwoFactorMethods.Remove(method);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}