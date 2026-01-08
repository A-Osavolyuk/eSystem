using eSecurity.Core.Security.Authentication.Lockout;
using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authentication.Lockout;

public sealed class LockoutManager(AuthDbContext context) : ILockoutManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<UserLockoutStateEntity?> GetAsync(UserEntity user,
        CancellationToken cancellationToken = default)
    {
        return await _context.LockoutStates.FirstOrDefaultAsync(
            x => x.UserId == user.Id, cancellationToken);
    }

    public async ValueTask<Result> BlockPermanentlyAsync(UserEntity user, LockoutType type,
        string? description = null, CancellationToken cancellationToken = default)
    {
        var state = await GetAsync(user, cancellationToken);
        if (state is null) return Results.NotFound("State not found");

        state.Permanent = true;
        state.Type = type;
        state.Description = description;
        state.StartDate = DateTimeOffset.UtcNow;

        _context.LockoutStates.Update(state);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> BlockTemporaryAsync(UserEntity user, LockoutType type, LockoutPeriod period,
        string? description = null, CancellationToken cancellationToken = default)
    {
        var state = await GetAsync(user, cancellationToken);
        if (state is null) return Results.NotFound("State not found");
        
        var duration = period switch
        {
            LockoutPeriod.Day => TimeSpan.FromDays(TimePeriods.Day),
            LockoutPeriod.Week => TimeSpan.FromDays(TimePeriods.Week),
            LockoutPeriod.Month => TimeSpan.FromDays(TimePeriods.Month),
            LockoutPeriod.Quarter => TimeSpan.FromDays(TimePeriods.Quarter),
            LockoutPeriod.Year => TimeSpan.FromDays(TimePeriods.Year),
            _ => throw new NotSupportedException("Not supported lockout period")
        };

        state.Permanent = false;
        state.Type = type;
        state.Description = description;
        state.EndDate = DateTimeOffset.UtcNow.Add(duration);
        state.StartDate = DateTimeOffset.UtcNow;

        _context.LockoutStates.Update(state);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> BlockTemporaryAsync(UserEntity user, LockoutType type, TimeSpan duration,
        string? description = null, CancellationToken cancellationToken = default)
    {
        var state = await GetAsync(user, cancellationToken);
        if (state is null) return Results.NotFound("State not found");

        state.Permanent = false;
        state.Type = type;
        state.Description = description;
        state.EndDate = DateTimeOffset.UtcNow.Add(duration);
        state.StartDate = DateTimeOffset.UtcNow;

        _context.LockoutStates.Update(state);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> UnblockAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        var state = await GetAsync(user, cancellationToken);
        if (state is null) return Results.NotFound("State not found");

        state.Permanent = false;
        state.Type = LockoutType.None;
        state.StartDate = DateTimeOffset.UtcNow;
        state.EndDate = DateTimeOffset.UtcNow;
        state.Description = null;

        _context.LockoutStates.Update(state);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }
}