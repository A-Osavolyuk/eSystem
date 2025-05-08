namespace eShop.Auth.Api.Services;

public class LockoutManager(AuthDbContext context) : ILockoutManager
{
    private readonly AuthDbContext context = context;
    
    public async ValueTask<LockoutStatus> GetStatusAsync(UserEntity user,
        CancellationToken cancellationToken = default)
    {
        var status = new LockoutStatus()
        {
            LockoutEnabled = user.LockoutEnabled,
            LockoutEnd = user.LockoutEnd
        };
        
        return await Task.FromResult(status);
    }
    
    public async ValueTask<Result> EnableAsync(UserEntity user, DateTimeOffset endDate, CancellationToken cancellationToken = default)
    {
        user.LockoutEnabled = true;
        user.UpdateDate = DateTime.UtcNow;
        user.LockoutEnd = endDate;
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> DisableAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        user.LockoutEnabled = false;
        user.UpdateDate = DateTime.UtcNow;
        user.LockoutEnd = null;
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
    
    public async ValueTask<Result> SetEndDateAsync(UserEntity user, DateTimeOffset endDate,
        CancellationToken cancellationToken = default)
    {
        user.LockoutEnd = endDate;
        user.UpdateDate = DateTime.UtcNow;
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}