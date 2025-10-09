namespace eShop.Auth.Api.Services;

[Injectable(typeof(ILockoutManager), ServiceLifetime.Scoped)]
public sealed class LockoutManager(AuthDbContext context) : ILockoutManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<Result> BlockAsync(UserEntity userEntity,
        LockoutType type, string? description = null, bool permanent = false,
        TimeSpan? duration = null, DateTimeOffset? endDate = null, CancellationToken cancellationToken = default)
    {
        var entity = await context.LockoutStates.FirstAsync(x => x.UserId == userEntity.Id, cancellationToken);
        
        var startDate = DateTimeOffset.UtcNow;

        if (duration.HasValue)
        {
            entity.EndDate = startDate.Add(duration.Value);
        }

        if (endDate.HasValue)
        {
            entity.EndDate = endDate.Value;
        }

        entity.StartDate = startDate;
        entity.Enabled = true;
        entity.Permanent = permanent;
        entity.Type = type;
        entity.Description = description;

        context.LockoutStates.Update(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> UnblockAsync(UserEntity userEntity, CancellationToken cancellationToken = default)
    {
        var entity = await context.LockoutStates.FirstAsync(x => x.UserId == userEntity.Id, cancellationToken);

        entity.Enabled = false;
        entity.Permanent = false;
        entity.UpdateDate = DateTimeOffset.UtcNow;
        entity.Type = LockoutType.None;
        entity.StartDate = DateTimeOffset.UtcNow;
        entity.EndDate = DateTimeOffset.UtcNow;
        entity.Description = null;

        context.LockoutStates.Update(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}