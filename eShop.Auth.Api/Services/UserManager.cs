using eShop.Auth.Api.Security.Hashing;

namespace eShop.Auth.Api.Services;

[Injectable(typeof(IUserManager), ServiceLifetime.Scoped)]
public sealed class UserManager(
    AuthDbContext context,
    Hasher hasher) : IUserManager
{
    private readonly AuthDbContext context = context;
    private readonly Hasher hasher = hasher;

    public async ValueTask<List<UserEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await context.Users
            .Include(x => x.Roles)
            .ThenInclude(x => x.Role)
            .Include(x => x.Permissions)
            .ThenInclude(x => x.Permission)
            .Include(x => x.TwoFactorProviders)
            .ThenInclude(x => x.TwoFactorProvider)
            .Include(x => x.LinkedAccounts)
            .ThenInclude(x => x.Provider)
            .Include(x => x.Changes)
            .Include(x => x.RecoveryCodes)
            .Include(x => x.PersonalData)
            .Include(x => x.LockoutState)
            .Include(x => x.Devices)
            .Include(x => x.Passkeys)
            .Include(x => x.Emails)
            .Include(x => x.PhoneNumbers)
            .ToListAsync(cancellationToken);

        return users;
    }

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
            .Include(x => x.TwoFactorProviders)
            .ThenInclude(x => x.TwoFactorProvider)
            .Include(x => x.LinkedAccounts)
            .ThenInclude(x => x.Provider)
            .Include(x => x.Changes)
            .Include(x => x.RecoveryCodes)
            .Include(x => x.PersonalData)
            .Include(x => x.LockoutState)
            .Include(x => x.Devices)
            .Include(x => x.Passkeys)
            .Include(x => x.PhoneNumbers)
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
            .Include(x => x.TwoFactorProviders)
            .ThenInclude(x => x.TwoFactorProvider)
            .Include(x => x.LinkedAccounts)
            .ThenInclude(x => x.Provider)
            .Include(x => x.Changes)
            .Include(x => x.RecoveryCodes)
            .Include(x => x.PersonalData)
            .Include(x => x.LockoutState)
            .Include(x => x.Devices)
            .Include(x => x.Passkeys)
            .Include(x => x.Emails)
            .Include(x => x.PhoneNumbers)
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
            .Include(x => x.TwoFactorProviders)
            .ThenInclude(x => x.TwoFactorProvider)
            .Include(x => x.LinkedAccounts)
            .ThenInclude(x => x.Provider)
            .Include(x => x.Changes)
            .Include(x => x.RecoveryCodes)
            .Include(x => x.PersonalData)
            .Include(x => x.LockoutState)
            .Include(x => x.Devices)
            .Include(x => x.Passkeys)
            .Include(x => x.Emails)
            .Include(x => x.PhoneNumbers)
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
            .Include(x => x.TwoFactorProviders)
            .ThenInclude(x => x.TwoFactorProvider)
            .Include(x => x.LinkedAccounts)
            .ThenInclude(x => x.Provider)
            .Include(x => x.Changes)
            .Include(x => x.RecoveryCodes)
            .Include(x => x.PersonalData)
            .Include(x => x.LockoutState)
            .Include(x => x.Devices)
            .Include(x => x.Passkeys)
            .Include(x => x.PhoneNumbers)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        return user;
    }

    public async ValueTask<Result> AsPrimaryAsync(UserEntity user, UserEmailEntity email,
        CancellationToken cancellationToken = default)
    {
        var primaryEmail = user.Emails.FirstOrDefault(x => x.Type == EmailType.Primary);
        if (primaryEmail is null) return Results.BadRequest("User does not have a primary email");

        primaryEmail.Type = EmailType.Secondary;
        primaryEmail.UpdateDate = DateTimeOffset.UtcNow;

        email.Type = EmailType.Primary;
        email.UpdateDate = DateTimeOffset.UtcNow;

        user.UpdateDate = DateTimeOffset.UtcNow;

        context.Users.Update(user);
        context.UserEmails.UpdateRange(primaryEmail, email);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> AsSecondaryAsync(UserEntity user, UserEmailEntity email,
        CancellationToken cancellationToken = default)
    {
        if (email.Type == EmailType.Primary)
            return Results.BadRequest("Cannot set the email primary email as secondary");

        email.Type = EmailType.Secondary;
        email.UpdateDate = DateTimeOffset.UtcNow;

        user.UpdateDate = DateTimeOffset.UtcNow;

        context.Users.Update(user);
        context.UserEmails.UpdateRange(email);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> AsRecoveryAsync(UserEntity user, UserEmailEntity email,
        CancellationToken cancellationToken = default)
    {
        var recoveryEmail = user.Emails.FirstOrDefault(x => x.Type == EmailType.Recovery);
        if (recoveryEmail is null) return Results.BadRequest("User does not have a recovery email");

        recoveryEmail.Type = EmailType.Secondary;
        recoveryEmail.UpdateDate = DateTimeOffset.UtcNow;

        email.Type = EmailType.Recovery;
        email.UpdateDate = DateTimeOffset.UtcNow;

        user.UpdateDate = DateTimeOffset.UtcNow;

        context.Users.Update(user);
        context.UserEmails.UpdateRange(recoveryEmail, email);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> AsPrimaryAsync(UserEntity user, UserPhoneNumberEntity phoneNumber,
        CancellationToken cancellationToken = default)
    {
        var primaryPhoneNumber = user.PhoneNumbers.FirstOrDefault(x => x.Type == PhoneNumberType.Primary);
        if (primaryPhoneNumber is null) return Results.BadRequest("User does not have a primary phone number");

        primaryPhoneNumber.Type = PhoneNumberType.Secondary;
        primaryPhoneNumber.UpdateDate = DateTimeOffset.UtcNow;

        phoneNumber.Type = PhoneNumberType.Primary;
        phoneNumber.UpdateDate = DateTimeOffset.UtcNow;

        user.UpdateDate = DateTimeOffset.UtcNow;

        context.Users.Update(user);
        context.UserPhoneNumbers.UpdateRange(primaryPhoneNumber, phoneNumber);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> AsSecondaryAsync(UserEntity user, UserPhoneNumberEntity phoneNumber,
        CancellationToken cancellationToken = default)
    {
        if (phoneNumber.Type == PhoneNumberType.Primary)
            return Results.BadRequest("Cannot set the primary phone number as secondary");

        phoneNumber.Type = PhoneNumberType.Secondary;
        phoneNumber.UpdateDate = DateTimeOffset.UtcNow;

        user.UpdateDate = DateTimeOffset.UtcNow;

        context.Users.Update(user);
        context.UserPhoneNumbers.UpdateRange(phoneNumber);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> AsRecoveryAsync(UserEntity user, UserPhoneNumberEntity phoneNumber,
        CancellationToken cancellationToken = default)
    {
        var recoveryPhoneNumber = user.PhoneNumbers.FirstOrDefault(x => x.Type == PhoneNumberType.Recovery);
        if (recoveryPhoneNumber is null) return Results.BadRequest("User does not have a primary phone number");

        recoveryPhoneNumber.Type = PhoneNumberType.Secondary;
        recoveryPhoneNumber.UpdateDate = DateTimeOffset.UtcNow;

        phoneNumber.Type = PhoneNumberType.Recovery;
        phoneNumber.UpdateDate = DateTimeOffset.UtcNow;

        user.UpdateDate = DateTimeOffset.UtcNow;

        context.Users.Update(user);
        context.UserPhoneNumbers.UpdateRange(recoveryPhoneNumber, phoneNumber);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> SetEmailAsync(UserEntity user, string email, EmailType type,
        CancellationToken cancellationToken = default)
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

    public async ValueTask<Result> ResetPasswordAsync(UserEntity user, string newPassword,
        CancellationToken cancellationToken = default)
    {
        var passwordHash = hasher.Hash(newPassword);

        user.PasswordHash = passwordHash;
        user.PasswordChangeDate = DateTimeOffset.UtcNow;
        user.UpdateDate = DateTimeOffset.UtcNow;

        context.Users.Update(user);
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

    public async ValueTask<Result> AddPasswordAsync(UserEntity user, string password,
        CancellationToken cancellationToken = default)
    {
        var passwordHash = hasher.Hash(password);

        user.PasswordHash = passwordHash;
        user.PasswordChangeDate = DateTimeOffset.UtcNow;
        user.UpdateDate = DateTimeOffset.UtcNow;

        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> CreateAsync(UserEntity user, string? password,
        CancellationToken cancellationToken = default)
    {
        var lockoutState = new LockoutStateEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            ReasonId = null,
            Enabled = false,
            CreateDate = DateTimeOffset.UtcNow,
        };

        var providers = await context.TwoFactorProviders.ToListAsync(cancellationToken);
        var userProviders = providers.Select(x => new UserTwoFactorProviderEntity()
        {
            UserId = user.Id,
            ProviderId = x.Id,
            CreateDate = DateTimeOffset.UtcNow,
            Subscribed = false
        }).ToList();

        if (!string.IsNullOrEmpty(password))
        {
            var passwordHash = hasher.Hash(password);
            user.PasswordHash = passwordHash;
        }

        user.NormalizedUsername = user.Username.ToUpper();
        user.CreateDate = DateTimeOffset.UtcNow;

        await context.Users.AddAsync(user, cancellationToken);
        await context.LockoutStates.AddAsync(lockoutState, cancellationToken);
        await context.UserTwoFactorProviders.AddRangeAsync(userProviders, cancellationToken);
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

    public bool CheckPassword(UserEntity user, string password)
    {
        return hasher.VerifyHash(password, user.PasswordHash);
    }

    public async ValueTask<Result> ChangePasswordAsync(UserEntity user,
        string newPassword, CancellationToken cancellationToken = default)
    {
        var newPasswordHash = hasher.Hash(newPassword);

        user.PasswordHash = newPasswordHash;
        user.PasswordChangeDate = DateTimeOffset.UtcNow;
        user.UpdateDate = DateTimeOffset.UtcNow;

        context.Users.Update(user);
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