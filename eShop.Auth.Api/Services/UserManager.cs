using eShop.Auth.Api.Security.Hashing;

namespace eShop.Auth.Api.Services;

[Injectable(typeof(IUserManager), ServiceLifetime.Scoped)]
public sealed class UserManager(
    AuthDbContext context,
    Hasher hasher,
    IChangeManager changeManager) : IUserManager
{
    private readonly AuthDbContext context = context;
    private readonly IChangeManager changeManager = changeManager;
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
            .ToListAsync(cancellationToken);

        return users;
    }

    public async ValueTask<UserEntity?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToUpper();
        var user = await context.Users.Where(x => x.NormalizedEmail == normalizedEmail)
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
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        return user;
    }

    public async ValueTask<UserEntity?> FindByPhoneNumberAsync(string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        var user = await context.Users.Where(x => x.PhoneNumber == phoneNumber)
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
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        return user;
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
        var result = await changeManager.CreateAsync(user,
            ChangeField.Password, user.PasswordHash, cancellationToken);

        if (!result.Succeeded)
        {
            return result;
        }

        var passwordHash = hasher.Hash(newPassword);

        user.PasswordHash = passwordHash;
        user.PasswordChangeDate = DateTimeOffset.UtcNow;
        user.UpdateDate = DateTimeOffset.UtcNow;

        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> ResetEmailAsync(UserEntity user,
        string newEmail, CancellationToken cancellationToken = default)
    {
        var result = await changeManager.CreateAsync(user,
            ChangeField.Email, user.Email, cancellationToken);

        if (!result.Succeeded)
        {
            return result;
        }

        user.Email = newEmail;
        user.EmailConfirmed = true;
        user.EmailConfirmationDate = DateTimeOffset.UtcNow;
        user.EmailChangeDate = DateTimeOffset.UtcNow;
        user.NormalizedEmail = newEmail.ToUpper();
        user.UpdateDate = DateTimeOffset.UtcNow;

        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> ResetRecoveryEmailAsync(UserEntity user, string newRecoveryEmail,
        CancellationToken cancellationToken = default)
    {
        var result = await changeManager.CreateAsync(user,
            ChangeField.RecoveryEmail, user.RecoveryEmail!, cancellationToken);

        if (!result.Succeeded)
        {
            return result;
        }

        user.RecoveryEmail = newRecoveryEmail;
        user.RecoveryEmailConfirmed = true;
        user.RecoveryEmailConfirmationDate = DateTimeOffset.UtcNow;
        user.NormalizedRecoveryEmail = newRecoveryEmail.ToUpper();
        user.RecoveryEmailChangeDate = DateTimeOffset.UtcNow;
        user.UpdateDate = DateTimeOffset.UtcNow;

        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> ResetPhoneNumberAsync(UserEntity user,
        string newPhoneNumber, CancellationToken cancellationToken = default)
    {
        var result = await changeManager.CreateAsync(user,
            ChangeField.PhoneNumber, user.PhoneNumber!, cancellationToken);

        if (!result.Succeeded)
        {
            return result;
        }

        user.PhoneNumber = newPhoneNumber;
        user.PhoneNumberConfirmed = true;
        user.PhoneNumberConfirmationDate = DateTimeOffset.UtcNow;
        user.PhoneNumberChangeDate = DateTimeOffset.UtcNow;
        user.UpdateDate = DateTimeOffset.UtcNow;

        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> RemovePhoneNumberAsync(UserEntity user,
        CancellationToken cancellationToken = default)
    {
        var result =
            await changeManager.CreateAsync(user,
                ChangeField.PhoneNumber, user.PhoneNumber!, cancellationToken);

        if (!result.Succeeded)
        {
            return result;
        }

        user.PhoneNumber = null;
        user.PhoneNumberConfirmed = false;
        user.PhoneNumberChangeDate = null;
        user.UpdateDate = DateTimeOffset.UtcNow;

        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> RemoveEmailAsync(UserEntity user, string email,
        CancellationToken cancellationToken = default)
    {
        var userEmail = await context.UserEmails.FirstOrDefaultAsync(
            x => x.Email == email && x.UserId == user.Id, cancellationToken);

        if (userEmail is null) return Results.NotFound($"User doesn't have email {email}");

        user.UpdateDate = DateTimeOffset.UtcNow;

        context.Users.Update(user);
        context.UserEmails.Remove(userEmail);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> RemoveRecoveryEmailAsync(UserEntity user,
        CancellationToken cancellationToken = default)
    {
        var result = await changeManager.CreateAsync(user,
            ChangeField.RecoveryEmail, user.PhoneNumber!, cancellationToken);

        if (!result.Succeeded)
        {
            return result;
        }

        user.RecoveryEmail = null;
        user.RecoveryEmailConfirmed = false;
        user.NormalizedRecoveryEmail = null;
        user.RecoveryEmailChangeDate = null;
        user.RecoveryEmailConfirmationDate = null;
        user.UpdateDate = DateTimeOffset.UtcNow;

        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> ChangeEmailAsync(UserEntity user, string currentEmail, string newEmail,
        CancellationToken cancellationToken = default)
    {
        var result = await changeManager.CreateAsync(user,
            ChangeField.Email, user.Email, cancellationToken);

        if (!result.Succeeded)
        {
            return result;
        }

        user.Email = newEmail;
        user.NormalizedEmail = newEmail.ToUpperInvariant();
        user.EmailChangeDate = DateTimeOffset.UtcNow;
        user.EmailConfirmationDate = DateTimeOffset.UtcNow;
        user.UpdateDate = DateTimeOffset.UtcNow;
        user.Username = newEmail;

        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> ChangePhoneNumberAsync(UserEntity user, string currentPhoneNumber,
        string newPhoneNumber, CancellationToken cancellationToken = default)
    {
        var result = await changeManager.CreateAsync(user,
            ChangeField.PhoneNumber, user.PhoneNumber!, cancellationToken);

        if (!result.Succeeded)
        {
            return result;
        }

        user.PhoneNumber = newPhoneNumber;
        user.UpdateDate = DateTimeOffset.UtcNow;
        user.PhoneNumberChangeDate = DateTimeOffset.UtcNow;
        user.PhoneNumberConfirmationDate = DateTimeOffset.UtcNow;

        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> ChangeRecoveryEmailAsync(UserEntity user, string newRecoveryEmail,
        CancellationToken cancellationToken = default)
    {
        var result = await changeManager.CreateAsync(user,
            ChangeField.RecoveryEmail, user.RecoveryEmail!, cancellationToken);

        if (!result.Succeeded)
        {
            return result;
        }

        user.RecoveryEmail = newRecoveryEmail;
        user.NormalizedRecoveryEmail = newRecoveryEmail.ToUpper();
        user.RecoveryEmailConfirmed = true;
        user.RecoveryEmailChangeDate = DateTimeOffset.UtcNow;
        user.RecoveryEmailConfirmationDate = DateTimeOffset.UtcNow;
        user.UpdateDate = DateTimeOffset.UtcNow;

        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> AddPhoneNumberAsync(UserEntity user,
        string phoneNumber, bool isPrimary = false, CancellationToken cancellationToken = default)
    {
        if (user.PhoneNumbers.Any(x => x.PhoneNumber == phoneNumber))
            return Results.BadRequest("User already has this phone number");

        var userPhoneNumber = new UserPhoneNumberEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            IsPrimary = isPrimary,
            PhoneNumber = phoneNumber,
            CreateDate = DateTimeOffset.UtcNow
        };

        user.UpdateDate = DateTimeOffset.UtcNow;

        context.Users.Update(user);
        await context.UserPhoneNumbers.AddAsync(userPhoneNumber, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> AddEmailAsync(UserEntity user, string email, bool isPrimary,
        bool isRecovery, CancellationToken cancellationToken = default)
    {
        if (user.Emails.Any(x => x.Email == email))
            return Results.BadRequest("User already has this email");

        var userEmail = new UserEmailEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            IsPrimary = isPrimary,
            IsRecovery = isRecovery,
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

        user.NormalizedEmail = user.Email.ToUpper();
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
        var result = await changeManager.CreateAsync(user, ChangeField.Username, user.Username, cancellationToken);

        if (!result.Succeeded)
        {
            return result;
        }

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

    public async ValueTask<bool> CheckPasswordAsync(UserEntity user,
        string password, CancellationToken cancellationToken = default)
    {
        var result = hasher.VerifyHash(password, user.PasswordHash);
        return await Task.FromResult(result);
    }

    public async ValueTask<Result> ChangePasswordAsync(UserEntity user,
        string newPassword, CancellationToken cancellationToken = default)
    {
        var result = await changeManager.CreateAsync(user, ChangeField.Password, user.PasswordHash, cancellationToken);

        if (!result.Succeeded)
        {
            return result;
        }

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
        var result = await context.Users.AnyAsync(
            u => u.NormalizedEmail == normalizedEmail
                 || u.NormalizedRecoveryEmail == normalizedEmail, cancellationToken);

        return result;
    }

    public async ValueTask<bool> IsPhoneNumberTakenAsync(string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        var result = await context.Users.AnyAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);
        return result;
    }
}