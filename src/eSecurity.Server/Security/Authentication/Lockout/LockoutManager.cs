using eSecurity.Core.Security.Authentication.Lockout;
using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

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
        if (state is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error()
            {
                Code = ErrorCode.NotFound,
                Description = "State not found"
            });
        }

        state.Permanent = true;
        state.Type = type;
        state.Description = description;
        state.StartedAt = DateTimeOffset.UtcNow;

        _context.LockoutStates.Update(state);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<Result> BlockTemporaryAsync(UserEntity user, LockoutType type, LockoutPeriod period,
        string? description = null, CancellationToken cancellationToken = default)
    {
        var state = await GetAsync(user, cancellationToken);
        if (state is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error()
            {
                Code = ErrorCode.NotFound,
                Description = "State not found"
            });
        }
        
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
        state.EndedAt = DateTimeOffset.UtcNow.Add(duration);
        state.StartedAt = DateTimeOffset.UtcNow;

        _context.LockoutStates.Update(state);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<Result> BlockTemporaryAsync(UserEntity user, LockoutType type, TimeSpan duration,
        string? description = null, CancellationToken cancellationToken = default)
    {
        var state = await GetAsync(user, cancellationToken);
        if (state is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error()
            {
                Code = ErrorCode.NotFound,
                Description = "State not found"
            });
        }

        state.Permanent = false;
        state.Type = type;
        state.Description = description;
        state.EndedAt = DateTimeOffset.UtcNow.Add(duration);
        state.StartedAt = DateTimeOffset.UtcNow;

        _context.LockoutStates.Update(state);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<Result> UnblockAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        var state = await GetAsync(user, cancellationToken);
        if (state is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error()
            {
                Code = ErrorCode.NotFound,
                Description = "State not found"
            });
        }

        state.Permanent = false;
        state.Type = LockoutType.None;
        state.StartedAt = DateTimeOffset.UtcNow;
        state.EndedAt = DateTimeOffset.UtcNow;
        state.Description = null;

        _context.LockoutStates.Update(state);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }
}