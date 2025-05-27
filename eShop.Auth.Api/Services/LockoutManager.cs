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
        bool permanent = false, DateTimeOffset? endDate = null, CancellationToken cancellationToken = default)
    {
        var entity = await context.LockoutState.FirstAsync(x => x.UserId == userEntity.Id, cancellationToken);

        entity.Permanent = permanent;
        entity.Enabled = true;
        entity.Reason = reason;
        entity.Description = description;
        entity.EndDate = endDate;
        entity.UpdateDate = DateTime.UtcNow;

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