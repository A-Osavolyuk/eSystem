using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Security.Identity.User;

public sealed class UserResendAttemptsService(AuthDbContext context) : IUserResendAttemptsService
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<Result> IncrementAttemptAsync(UserEntity user, TimeSpan dueTime,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        
        user.ResendAttempts += 1;
        user.ResendAvailableAt = DateTimeOffset.UtcNow.Add(dueTime);

        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<Result> ResetAttemptsAsync(UserEntity user, TimeSpan dueTime,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
       
        user.ResendAttempts += 0;
        user.ResendAvailableAt = DateTimeOffset.UtcNow.Add(dueTime);

        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<Result> CleanAttemptsAsync(UserEntity user, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);

        user.ResendAttempts += 0;
        user.ResendAvailableAt = null;

        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Success(SuccessCodes.Ok);
    }
}