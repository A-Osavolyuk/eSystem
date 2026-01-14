using eSecurity.Core.Security.Authentication.Lockout;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Identity.User;

public sealed class UserManager(AuthDbContext context) : IUserManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<UserEntity?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToUpper();
        var user = await _context.Users
            .FirstOrDefaultAsync(u => _context.UserEmails
                .Any(e => e.UserId == u.Id &&
                          e.Type == EmailType.Primary &&
                          e.NormalizedEmail == normalizedEmail), cancellationToken);

        return user;
    }

    public async ValueTask<UserEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        return user;
    }

    public async ValueTask<UserEntity?> FindByUsernameAsync(string name, CancellationToken cancellationToken = default)
    {
        var normalizedUserName = name.ToUpper();
        var user = await _context.Users.Where(x => x.NormalizedUsername == normalizedUserName)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        return user;
    }

    public async ValueTask<UserEntity?> FindByPhoneNumberAsync(string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .Where(x => _context.UserPhoneNumbers
                .Any(e => e.UserId == x.Id && e.PhoneNumber == phoneNumber))
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        return user;
    }

    public async ValueTask<Result> CreateAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        var lockoutState = new UserLockoutStateEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Type = LockoutType.None,
        };

        user.NormalizedUsername = user.Username.ToUpper();

        await _context.Users.AddAsync(user, cancellationToken);
        await _context.LockoutStates.AddAsync(lockoutState, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> UpdateAsync(UserEntity user,
        CancellationToken cancellationToken = default)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> DeleteAsync(UserEntity user,
        CancellationToken cancellationToken = default)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }
}