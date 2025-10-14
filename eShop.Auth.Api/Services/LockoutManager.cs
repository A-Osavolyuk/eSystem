using eShop.Auth.Api.Types;

namespace eShop.Auth.Api.Services;

[Injectable(typeof(ILockoutManager), ServiceLifetime.Scoped)]
public sealed class LockoutManager(AuthDbContext context) : ILockoutManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<Result> BlockPermanentlyAsync(UserEntity user, LockoutType type, 
         string? description = null, CancellationToken cancellationToken = default)
    {
        var state = user.LockoutState;
        
        state.Permanent = true;
        state.Type = type;
        state.Description = description;
        state.EndDate = null;
        state.StartDate = DateTimeOffset.UtcNow;
        
        context.LockoutStates.Update(state);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> BlockTemporaryAsync(UserEntity user, LockoutType type, LockoutPeriod period, 
        string? description = null, CancellationToken cancellationToken = default)
    {
        var duration = period switch
        {
            LockoutPeriod.Day => TimeSpan.FromDays(TimePeriods.Day),
            LockoutPeriod.Week => TimeSpan.FromDays(TimePeriods.Week),
            LockoutPeriod.Month => TimeSpan.FromDays(TimePeriods.Month),
            LockoutPeriod.Quarter => TimeSpan.FromDays(TimePeriods.Quarter),
            LockoutPeriod.Year => TimeSpan.FromDays(TimePeriods.Year),
            _ => throw new NotSupportedException("Not supported lockout period")
        };
        
        var state = user.LockoutState;
        var startDate = DateTimeOffset.UtcNow;
        
        state.Permanent = false;
        state.Type = type;
        state.Description = description;
        state.EndDate = startDate.Add(duration);
        state.StartDate = startDate;
        
        context.LockoutStates.Update(state);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
    
    public async ValueTask<Result> BlockTemporaryAsync(UserEntity user, LockoutType type, TimeSpan duration, 
        string? description = null, CancellationToken cancellationToken = default)
    {
        var state = user.LockoutState;
        var startDate = DateTimeOffset.UtcNow;
        
        state.Permanent = false;
        state.Type = type;
        state.Description = description;
        state.EndDate = startDate.Add(duration);
        state.StartDate = startDate;
        
        context.LockoutStates.Update(state);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> UnblockAsync(UserEntity userEntity, CancellationToken cancellationToken = default)
    {
        var entity = await context.LockoutStates.FirstAsync(x => x.UserId == userEntity.Id, cancellationToken);
        
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