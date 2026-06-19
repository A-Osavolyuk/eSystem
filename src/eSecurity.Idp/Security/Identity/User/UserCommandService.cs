using eSecurity.Core.Security.Authentication.Lockout;
using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Security.Identity.User;

public sealed class UserCommandService(AuthDbContext context) : IUserCommandService
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<Result> CreateAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);

        var lockoutState = new UserLockoutStateEntity
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Type = LockoutType.None,
        };
        
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.LockoutStates.AddAsync(lockoutState, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Success(SuccessCodes.Ok);
    }
}