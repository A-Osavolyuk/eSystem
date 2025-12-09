using eSecurity.Core.Security.Authentication.Lockout;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Identity.User;

public sealed class UserManager(AuthDbContext context) : IUserManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<UserEntity?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToUpper();
        var user = await _context.Users
            .Where(x => _context.UserEmails
                .Any(e => e.UserId == e.Id && e.Type == EmailType.Primary))
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

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

    public async ValueTask<Result> SetUsernameAsync(UserEntity user, string username,
        CancellationToken cancellationToken = default)
    {
        user.Username = username;
        user.UsernameChangeDate = DateTimeOffset.UtcNow;
        user.UpdateDate = DateTimeOffset.UtcNow;

        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> CreateAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        var lockoutState = new UserLockoutStateEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Type = LockoutType.None,
            CreateDate = DateTimeOffset.UtcNow,
        };

        user.NormalizedUsername = user.Username.ToUpper();
        user.CreateDate = DateTimeOffset.UtcNow;

        await _context.Users.AddAsync(user, cancellationToken);
        await _context.LockoutStates.AddAsync(lockoutState, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> UpdateAsync(UserEntity user,
        CancellationToken cancellationToken = default)
    {
        user.UpdateDate = DateTimeOffset.UtcNow;

        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> ChangeUsernameAsync(UserEntity user, string userName,
        CancellationToken cancellationToken = default)
    {
        user.Username = userName;
        user.NormalizedUsername = userName.ToUpper();
        user.UsernameChangeDate = DateTimeOffset.UtcNow;
        user.UpdateDate = DateTimeOffset.UtcNow;

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

    public async ValueTask<bool> IsUsernameTakenAsync(string userName,
        CancellationToken cancellationToken = default)
    {
        var normalizedUserName = userName.ToUpper();
        var result = await _context.Users.AnyAsync(
            u => u.NormalizedUsername == normalizedUserName, cancellationToken);

        return result;
    }
}