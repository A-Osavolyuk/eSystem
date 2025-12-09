using eSecurity.Core.Security.Authentication.Lockout;
using eSecurity.Core.Security.Authorization.Access;
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
            .Include(x => x.Emails)
            .Where(x => x.Emails.Any(e => e.NormalizedEmail == normalizedEmail && e.Type == EmailType.Primary))
            .Include(x => x.Roles)
            .ThenInclude(x => x.Role)
            .Include(x => x.Permissions)
            .ThenInclude(x => x.Permission)
            .Include(x => x.TwoFactorMethods)
            .Include(x => x.LinkedAccounts)
            .Include(x => x.RecoveryCodes)
            .Include(x => x.PersonalData)
            .Include(x => x.LockoutState)
            .Include(x => x.Devices)
            .ThenInclude(x => x.Passkey)
            .Include(x => x.PhoneNumbers)
            .Include(x => x.Secret)
            .Include(x => x.Password)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        return user;
    }

    public async ValueTask<UserEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.Where(x => x.Id == id)
            .Include(x => x.Roles)
            .ThenInclude(x => x.Role)
            .Include(x => x.Permissions)
            .ThenInclude(x => x.Permission)
            .Include(x => x.TwoFactorMethods)
            .Include(x => x.LinkedAccounts)
            .Include(x => x.RecoveryCodes)
            .Include(x => x.PersonalData)
            .Include(x => x.LockoutState)
            .Include(x => x.Devices)
            .ThenInclude(x => x.Passkey)
            .Include(x => x.Emails)
            .Include(x => x.PhoneNumbers)
            .Include(x => x.Secret)
            .Include(x => x.Password)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        return user;
    }

    public async ValueTask<UserEntity?> FindByUsernameAsync(string name, CancellationToken cancellationToken = default)
    {
        var normalizedUserName = name.ToUpper();
        var user = await _context.Users.Where(x => x.NormalizedUsername == normalizedUserName)
            .Include(x => x.Roles)
            .ThenInclude(x => x.Role)
            .Include(x => x.Permissions)
            .ThenInclude(x => x.Permission)
            .Include(x => x.TwoFactorMethods)
            .Include(x => x.LinkedAccounts)
            .Include(x => x.RecoveryCodes)
            .Include(x => x.PersonalData)
            .Include(x => x.LockoutState)
            .Include(x => x.Devices)
            .ThenInclude(x => x.Passkey)
            .Include(x => x.Emails)
            .Include(x => x.PhoneNumbers)
            .Include(x => x.Secret)
            .Include(x => x.Password)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        return user;
    }

    public async ValueTask<UserEntity?> FindByPhoneNumberAsync(string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .Include(x => x.PhoneNumbers)
            .Where(x => x.PhoneNumbers.Any(p => p.PhoneNumber == phoneNumber && p.Type == PhoneNumberType.Primary))
            .Include(x => x.Roles)
            .ThenInclude(x => x.Role)
            .Include(x => x.Permissions)
            .ThenInclude(x => x.Permission)
            .Include(x => x.TwoFactorMethods)
            .Include(x => x.LinkedAccounts)
            .Include(x => x.RecoveryCodes)
            .Include(x => x.PersonalData)
            .Include(x => x.LockoutState)
            .Include(x => x.Devices)
            .ThenInclude(x => x.Passkey)
            .Include(x => x.PhoneNumbers)
            .Include(x => x.Secret)
            .Include(x => x.Password)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        return user;
    }

    public async ValueTask<Result> SetPhoneNumberAsync(UserEntity user, string phoneNumber, PhoneNumberType type,
        CancellationToken cancellationToken = default)
    {
        if (user.PhoneNumbers.Any(x => x.PhoneNumber == phoneNumber))
            return Results.BadRequest("User already has this phone number");

        var userPhoneNumber = new UserPhoneNumberEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            PhoneNumber = phoneNumber,
            Type = type,
            IsVerified = true,
            VerifiedDate = DateTimeOffset.UtcNow,
            CreateDate = DateTimeOffset.UtcNow
        };

        user.UpdateDate = DateTimeOffset.UtcNow;

        _context.Users.Update(user);
        await _context.UserPhoneNumbers.AddAsync(userPhoneNumber, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
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

    public async ValueTask<Result> VerifyPhoneNumberAsync(UserEntity user, string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        var userPhoneNumber = user.PhoneNumbers.FirstOrDefault(x => x.PhoneNumber == phoneNumber);
        if (userPhoneNumber == null) return Results.NotFound($"User doesn't have phone number {phoneNumber}");

        userPhoneNumber.IsVerified = true;
        userPhoneNumber.VerifiedDate = DateTimeOffset.UtcNow;
        userPhoneNumber.UpdateDate = DateTimeOffset.UtcNow;
        user.UpdateDate = DateTimeOffset.UtcNow;

        _context.Users.Update(user);
        _context.UserPhoneNumbers.Update(userPhoneNumber);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> ResetPhoneNumberAsync(UserEntity user, string currentPhoneNumber,
        string newPhoneNumber, CancellationToken cancellationToken = default)
    {
        var userPhoneNumber = user.PhoneNumbers.FirstOrDefault(x => x.PhoneNumber == currentPhoneNumber);
        if (userPhoneNumber is null) return Results.NotFound($"User doesn't have phone number {currentPhoneNumber}");

        userPhoneNumber.PhoneNumber = newPhoneNumber;
        userPhoneNumber.UpdateDate = DateTimeOffset.UtcNow;
        userPhoneNumber.IsVerified = true;
        userPhoneNumber.VerifiedDate = DateTimeOffset.UtcNow;
        user.UpdateDate = DateTimeOffset.UtcNow;

        _context.Users.Update(user);
        _context.UserPhoneNumbers.Update(userPhoneNumber);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> RemovePhoneNumberAsync(UserEntity user, string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        var userPhoneNumber = user.PhoneNumbers.FirstOrDefault(x => x.PhoneNumber == phoneNumber);
        if (userPhoneNumber == null) return Results.NotFound($"User doesn't have phone number {phoneNumber}");

        user.UpdateDate = DateTimeOffset.UtcNow;

        _context.Users.Update(user);
        _context.UserPhoneNumbers.Remove(userPhoneNumber);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> ChangePhoneNumberAsync(UserEntity user, string currentPhoneNumber,
        string newPhoneNumber, CancellationToken cancellationToken = default)
    {
        var userPhoneNumber = user.PhoneNumbers.FirstOrDefault(x => x.PhoneNumber == currentPhoneNumber);
        if (userPhoneNumber is null) return Results.NotFound($"User doesn't have phone number {currentPhoneNumber}");

        userPhoneNumber.PhoneNumber = newPhoneNumber;
        userPhoneNumber.UpdateDate = DateTimeOffset.UtcNow;
        userPhoneNumber.IsVerified = true;
        userPhoneNumber.VerifiedDate = DateTimeOffset.UtcNow;
        user.UpdateDate = DateTimeOffset.UtcNow;

        _context.Users.Update(user);
        _context.UserPhoneNumbers.Update(userPhoneNumber);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> AddPhoneNumberAsync(UserEntity user, string phoneNumber,
        PhoneNumberType type, CancellationToken cancellationToken = default)
    {
        if (user.PhoneNumbers.Any(x => x.PhoneNumber == phoneNumber))
            return Results.BadRequest("User already has this phone number");

        var userPhoneNumber = new UserPhoneNumberEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Type = type,
            PhoneNumber = phoneNumber,
            CreateDate = DateTimeOffset.UtcNow
        };

        user.UpdateDate = DateTimeOffset.UtcNow;

        _context.Users.Update(user);
        await _context.UserPhoneNumbers.AddAsync(userPhoneNumber, cancellationToken);
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

    public async ValueTask<bool> IsPhoneNumberTakenAsync(string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        var result = await _context.UserPhoneNumbers
            .AnyAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);

        return result;
    }
}