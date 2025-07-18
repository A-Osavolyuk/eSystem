namespace eShop.Auth.Api.Services;

[Injectable(typeof(ILockoutManager), ServiceLifetime.Scoped)]
public sealed class LockoutManager(AuthDbContext context) : ILockoutManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<LockoutStateEntity> FindAsync(UserEntity userEntity,
        CancellationToken cancellationToken = default)
    {
        var entity = await context.LockoutStates.FirstAsync(x => x.UserId == userEntity.Id, cancellationToken);
        return entity;
    }

    public async ValueTask<Result> LockoutAsync(UserEntity userEntity,
        LockoutReasonEntity lockoutReason, string? description = null, bool permanent = false,
        TimeSpan? duration = null, DateTimeOffset? endDate = null, CancellationToken cancellationToken = default)
    {
        var entity = await context.LockoutStates.FirstAsync(x => x.UserId == userEntity.Id, cancellationToken);
        
        var startDate = DateTimeOffset.UtcNow;

        if (duration.HasValue)
        {
            entity.Duration = duration.Value;
            entity.EndDate = startDate.Add(duration.Value);
        }

        entity.StartDate = startDate;
        entity.Enabled = true;
        entity.Permanent = permanent;
        entity.ReasonId = lockoutReason.Id;
        entity.Description = description ?? lockoutReason.Description;

        context.LockoutStates.Update(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> UnlockAsync(UserEntity userEntity, CancellationToken cancellationToken = default)
    {
        var entity = await context.LockoutStates.FirstAsync(x => x.UserId == userEntity.Id, cancellationToken);

        entity.Enabled = false;
        entity.Permanent = false;
        entity.UpdateDate = DateTimeOffset.UtcNow;
        entity.ReasonId = null;
        entity.Reason = null;
        entity.StartDate = DateTimeOffset.UtcNow;
        entity.EndDate = DateTimeOffset.UtcNow;
        entity.Duration = null;
        entity.Description = null;

        context.LockoutStates.Update(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}