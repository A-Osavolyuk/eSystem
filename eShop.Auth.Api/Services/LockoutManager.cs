namespace eShop.Auth.Api.Services;

public class LockoutManager(AuthDbContext context) : ILockoutManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<LockoutStateEntity> FindAsync(UserEntity userEntity,
        CancellationToken cancellationToken = default)
    {
        var entity = await context.LockoutState.FirstAsync(x => x.UserId == userEntity.Id, cancellationToken);
        return entity;
    }

    public async ValueTask<Result> LockoutAsync(UserEntity userEntity, LockoutReason reason, string description,
        LockoutPeriod period, DateTimeOffset? endDate = null, CancellationToken cancellationToken = default)
    {
        var entity = await context.LockoutState.FirstAsync(x => x.UserId == userEntity.Id, cancellationToken);

        entity.Enabled = true;
        entity.Reason = reason;
        entity.Description = description;
        entity.UpdateDate = DateTime.UtcNow;
        entity.Permanent = period is LockoutPeriod.Permanent;
        entity.EndDate = period switch
        {
            LockoutPeriod.Day => DateTimeOffset.UtcNow.AddDays(1),
            LockoutPeriod.Week => DateTimeOffset.UtcNow.AddDays(7),
            LockoutPeriod.Month => DateTimeOffset.UtcNow.AddMonths(1),
            LockoutPeriod.Quarter => DateTimeOffset.UtcNow.AddMonths(3),
            LockoutPeriod.Year => DateTimeOffset.UtcNow.AddYears(1),
            LockoutPeriod.Permanent or LockoutPeriod.None => null,
            LockoutPeriod.Custom => endDate,
            _ => throw new NotSupportedException("Not supported period")
        };

        context.LockoutState.Update(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> UnlockAsync(UserEntity userEntity, CancellationToken cancellationToken = default)
    {
        var entity = await context.LockoutState.FirstAsync(x => x.UserId == userEntity.Id, cancellationToken);

        entity.Enabled = false;
        entity.Reason = LockoutReason.None;
        entity.Description = string.Empty;
        entity.EndDate = null;
        entity.UpdateDate = DateTime.UtcNow;

        context.LockoutState.Update(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}