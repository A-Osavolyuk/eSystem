using eSecurity.Data.Entities;
using eSecurity.Security.Authentication.Lockout;
using eSecurity.Security.Authorization.Access;
using eSecurity.Security.Authorization.Access.Verification;

namespace eSecurity.Security.Identity.User;

public sealed class UserManager(AuthDbContext context) : IUserManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<UserEntity?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToUpper();
        var user = await context.Users
            .Include(x => x.Emails)
            .Where(x => x.Emails.Any(e => e.NormalizedEmail == normalizedEmail && e.Type == EmailType.Primary))
            .Include(x => x.Roles)
            .ThenInclude(x => x.Role)
            .Include(x => x.Permissions)
            .ThenInclude(x => x.Permission)
            .Include(x => x.TwoFactorMethods)
            .Include(x => x.LinkedAccounts)
            .Include(x => x.VerificationMethods)
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
        var user = await context.Users.Where(x => x.Id == id)
            .Include(x => x.Roles)
            .ThenInclude(x => x.Role)
            .Include(x => x.Permissions)
            .ThenInclude(x => x.Permission)
            .Include(x => x.TwoFactorMethods)
            .Include(x => x.LinkedAccounts)
            .Include(x => x.VerificationMethods)
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
        var user = await context.Users.Where(x => x.NormalizedUsername == normalizedUserName)
            .Include(x => x.Roles)
            .ThenInclude(x => x.Role)
            .Include(x => x.Permissions)
            .ThenInclude(x => x.Permission)
            .Include(x => x.TwoFactorMethods)
            .Include(x => x.LinkedAccounts)
            .Include(x => x.VerificationMethods)
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
        var user = await context.Users
            .Include(x => x.PhoneNumbers)
            .Where(x => x.PhoneNumbers.Any(p => p.PhoneNumber == phoneNumber && p.Type == PhoneNumberType.Primary))
            .Include(x => x.Roles)
            .ThenInclude(x => x.Role)
            .Include(x => x.Permissions)
            .ThenInclude(x => x.Permission)
            .Include(x => x.TwoFactorMethods)
            .Include(x => x.LinkedAccounts)
            .Include(x => x.VerificationMethods)
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

    public async ValueTask<Result> SetEmailAsync(UserEntity user, string email, EmailType type,
        CancellationToken cancellationToken = default)
    {
        var userEmail = new UserEmailEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            Type = type,
            IsVerified = true,
            VerifiedDate = DateTimeOffset.UtcNow,
            CreateDate = DateTimeOffset.UtcNow
        };

        user.UpdateDate = DateTimeOffset.UtcNow;

        context.Users.Update(user);
        await context.UserEmails.AddAsync(userEmail, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
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

        context.Users.Update(user);
        await context.UserPhoneNumbers.AddAsync(userPhoneNumber, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> SetUsernameAsync(UserEntity user, string username,
        CancellationToken cancellationToken = default)
    {
        user.Username = username;
        user.UsernameChangeDate = DateTimeOffset.UtcNow;
        user.UpdateDate = DateTimeOffset.UtcNow;

        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> ManageEmailAsync(UserEntity user, EmailType type, string email,
        CancellationToken cancellationToken = default)
    {
        var currentEmail = user.GetEmail(type);
        if (currentEmail is not null)
        {
            currentEmail.Type = EmailType.Secondary;
            currentEmail.UpdateDate = DateTimeOffset.UtcNow;
            context.UserEmails.Update(currentEmail);
        }

        var nextEmail = user.GetEmail(email);
        if (nextEmail is null) return Results.BadRequest($"User doesn't have email {email}.");

        nextEmail.Type = type;
        nextEmail.UpdateDate = DateTimeOffset.UtcNow;

        context.UserEmails.Update(nextEmail);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> VerifyEmailAsync(UserEntity user, string email,
        CancellationToken cancellationToken = default)
    {
        var userEmail = user.Emails.FirstOrDefault(x => x.Email == email);
        if (userEmail == null) return Results.NotFound($"User doesn't have email {email}");

        userEmail.IsVerified = true;
        userEmail.VerifiedDate = DateTimeOffset.UtcNow;
        userEmail.CreateDate = DateTimeOffset.UtcNow;
        user.UpdateDate = DateTimeOffset.UtcNow;

        context.Users.Update(user);
        context.UserEmails.Update(userEmail);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
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

        context.Users.Update(user);
        context.UserPhoneNumbers.Update(userPhoneNumber);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> ResetEmailAsync(UserEntity user, string currentEmail,
        string newEmail, CancellationToken cancellationToken = default)
    {
        var userEmail = user.Emails.FirstOrDefault(x => x.Email == currentEmail);
        if (userEmail is null) return Results.NotFound($"User doesn't have email {currentEmail}");

        userEmail.Email = newEmail;
        userEmail.NormalizedEmail = newEmail.ToUpperInvariant();
        userEmail.IsVerified = true;
        userEmail.VerifiedDate = DateTimeOffset.UtcNow;
        userEmail.UpdateDate = DateTimeOffset.UtcNow;

        user.UpdateDate = DateTimeOffset.UtcNow;

        context.Users.Update(user);
        context.UserEmails.Update(userEmail);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
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

        context.Users.Update(user);
        context.UserPhoneNumbers.Update(userPhoneNumber);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> RemovePhoneNumberAsync(UserEntity user, string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        var userPhoneNumber = user.PhoneNumbers.FirstOrDefault(x => x.PhoneNumber == phoneNumber);
        if (userPhoneNumber == null) return Results.NotFound($"User doesn't have phone number {phoneNumber}");

        user.UpdateDate = DateTimeOffset.UtcNow;

        context.Users.Update(user);
        context.UserPhoneNumbers.Remove(userPhoneNumber);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> RemoveEmailAsync(UserEntity user, string email,
        CancellationToken cancellationToken = default)
    {
        var userEmail = user.Emails.First(x => x.Email == email);

        user.UpdateDate = DateTimeOffset.UtcNow;

        context.Users.Update(user);
        context.UserEmails.Remove(userEmail);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> ChangeEmailAsync(UserEntity user, string currentEmail, string newEmail,
        CancellationToken cancellationToken = default)
    {
        var userEmail = user.Emails.FirstOrDefault(x => x.Email == currentEmail);
        if (userEmail is null) return Results.NotFound($"User doesn't have email {currentEmail}");

        userEmail.Email = newEmail;
        userEmail.NormalizedEmail = newEmail.ToUpperInvariant();
        userEmail.IsVerified = true;
        userEmail.VerifiedDate = DateTimeOffset.UtcNow;
        userEmail.UpdateDate = DateTimeOffset.UtcNow;

        user.UpdateDate = DateTimeOffset.UtcNow;

        context.Users.Update(user);
        context.UserEmails.Update(userEmail);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
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

        context.Users.Update(user);
        context.UserPhoneNumbers.Update(userPhoneNumber);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
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

        context.Users.Update(user);
        await context.UserPhoneNumbers.AddAsync(userPhoneNumber, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> AddEmailAsync(UserEntity user, string email,
        EmailType type, CancellationToken cancellationToken = default)
    {
        if (user.Emails.Any(x => x.Email == email))
            return Results.BadRequest("User already has this email");

        var userEmail = new UserEmailEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            Type = type,
            CreateDate = DateTimeOffset.UtcNow
        };

        user.UpdateDate = DateTimeOffset.UtcNow;

        context.Users.Update(user);
        await context.UserEmails.AddAsync(userEmail, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
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

        var verificationMethod = new UserVerificationMethodEntity()
        {
            UserId = user.Id,
            Id = Guid.CreateVersion7(),
            Method = VerificationMethod.Email,
            Preferred = true,
            CreateDate = DateTimeOffset.UtcNow
        };

        user.NormalizedUsername = user.Username.ToUpper();
        user.CreateDate = DateTimeOffset.UtcNow;

        await context.Users.AddAsync(user, cancellationToken);
        await context.LockoutStates.AddAsync(lockoutState, cancellationToken);
        await context.UserVerificationMethods.AddAsync(verificationMethod, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> UpdateAsync(UserEntity user,
        CancellationToken cancellationToken = default)
    {
        user.UpdateDate = DateTimeOffset.UtcNow;

        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> ChangeUsernameAsync(UserEntity user, string userName,
        CancellationToken cancellationToken = default)
    {
        user.Username = userName;
        user.NormalizedUsername = userName.ToUpper();
        user.UsernameChangeDate = DateTimeOffset.UtcNow;
        user.UpdateDate = DateTimeOffset.UtcNow;

        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> DeleteAsync(UserEntity user,
        CancellationToken cancellationToken = default)
    {
        context.Users.Remove(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<bool> IsUsernameTakenAsync(string userName,
        CancellationToken cancellationToken = default)
    {
        var normalizedUserName = userName.ToUpper();
        var result = await context.Users.AnyAsync(
            u => u.NormalizedUsername == normalizedUserName, cancellationToken);

        return result;
    }

    public async ValueTask<bool> IsEmailTakenAsync(string email,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToUpper();
        var result = await context.UserEmails.AnyAsync(
            u => u.NormalizedEmail == normalizedEmail, cancellationToken);

        return result;
    }

    public async ValueTask<bool> IsPhoneNumberTakenAsync(string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        var result = await context.UserPhoneNumbers
            .AnyAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);

        return result;
    }
}