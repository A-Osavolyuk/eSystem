using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Security.Identity.User;

public sealed class UserFailedLoginService(AuthDbContext context) : IUserFailedLoginService
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<Result> ResetAttemptsAsync(UserEntity userEntity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userEntity);

        if (userEntity.FailedLoginAttempts == 0)
            return Results.Success(SuccessCodes.Ok);

        userEntity.FailedLoginAttempts = 0;
        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<Result> IncrementAttemptAsync(UserEntity userEntity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userEntity);

        userEntity.FailedLoginAttempts += 1;
        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Success(SuccessCodes.Ok);
    }
}