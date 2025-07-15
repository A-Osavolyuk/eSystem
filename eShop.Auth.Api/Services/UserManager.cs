namespace eShop.Auth.Api.Services;

[Injectable(typeof(IUserManager), ServiceLifetime.Scoped)]
public sealed class UserManager(AuthDbContext context) : IUserManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<List<UserEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await context.Users.ToListAsync(cancellationToken);
        return users;
    }

    public async ValueTask<UserEntity?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken: cancellationToken);
        return user;
    }

    public async ValueTask<UserEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);
        return user;
    }

    public async ValueTask<UserEntity?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == name,
            cancellationToken: cancellationToken);
        return user;
    }

    public async ValueTask<UserEntity?> FindByPhoneNumberAsync(string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber,
            cancellationToken: cancellationToken);
        return user;
    }

    public async ValueTask<Result> ConfirmEmailAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        user.EmailConfirmed = true;
        user.UpdateDate = DateTimeOffset.UtcNow;
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> ConfirmRecoveryEmailAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        user.RecoveryEmailConfirmed = true;
        user.UpdateDate = DateTimeOffset.UtcNow;
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> ConfirmPhoneNumberAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        user.PhoneNumberConfirmed = true;
        user.UpdateDate = DateTimeOffset.UtcNow;
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> ResetPasswordAsync(UserEntity user, string newPassword,
        CancellationToken cancellationToken = default)
    {
        var passwordHash = PasswordHasher.HashPassword(newPassword);

        user.PasswordHash = passwordHash;
        user.PasswordChangeDate = DateTimeOffset.UtcNow;
        user.UpdateDate = DateTimeOffset.UtcNow;
        
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> ResetEmailAsync(UserEntity user, string newEmail, CancellationToken cancellationToken = default)
    {
        user.Email = newEmail;
        user.EmailConfirmed = true;
        user.EmailChangeDate = DateTimeOffset.UtcNow;
        user.NormalizedEmail = newEmail.ToUpper();
        user.UpdateDate = DateTimeOffset.UtcNow;
        
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> ResetPhoneNumberAsync(UserEntity user, string newPhoneNumber, CancellationToken cancellationToken = default)
    {
        user.PhoneNumber = newPhoneNumber;
        user.PhoneNumberConfirmed = true;
        user.PhoneNumberChangeDate = DateTimeOffset.UtcNow;
        user.UpdateDate = DateTimeOffset.UtcNow;
        
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> ChangeEmailAsync(UserEntity user, string newEmail, CancellationToken cancellationToken = default)
    {
        user.Email = newEmail;
        user.NormalizedEmail = newEmail.ToUpperInvariant();
        user.EmailChangeDate = DateTimeOffset.UtcNow;
        user.UpdateDate = DateTimeOffset.UtcNow;
        user.UserName = newEmail;
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> ChangePhoneNumberAsync(UserEntity user, string newPhoneNumber, CancellationToken cancellationToken = default)
    {
        user.PhoneNumber = newPhoneNumber;
        user.UpdateDate = DateTimeOffset.UtcNow;
        user.PhoneNumberChangeDate = DateTimeOffset.UtcNow;
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> AddPhoneNumberAsync(UserEntity user, string phoneNumber, CancellationToken cancellationToken = default)
    {
        user.PhoneNumber = phoneNumber;
        user.UpdateDate = DateTimeOffset.UtcNow;
        
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> AddRecoveryEmailAsync(UserEntity user, string recoveryEmail, CancellationToken cancellationToken = default)
    {
        user.RecoveryEmail = recoveryEmail;
        user.NormalizedRecoveryEmail = recoveryEmail.ToUpperInvariant();
        user.RecoveryEmailChangeDate = null;
        user.UpdateDate = DateTimeOffset.UtcNow;
        
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> CreateAsync(UserEntity user, string password,
        CancellationToken cancellationToken = default)
    {
        var lockoutState = new LockoutStateEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Reason = LockoutReason.None,
            Enabled = false,
            CreateDate = DateTimeOffset.UtcNow,
        };
        
        var providers = await context.Providers
            .Select(p => new UserProviderEntity()
            {
                UserId = user.Id, 
                ProviderId = p.Id, 
                Subscribed = false, 
                CreateDate = DateTimeOffset.UtcNow
            })
            .ToListAsync(cancellationToken);

        var passwordHash = PasswordHasher.HashPassword(password);

        user.PasswordHash = passwordHash;
        user.NormalizedEmail = user.Email.ToUpper();
        user.NormalizedUserName = user.UserName.ToUpper();
        user.CreateDate = DateTimeOffset.UtcNow;
        
        await context.Users.AddAsync(user, cancellationToken);
        await context.UserProvider.AddRangeAsync(providers, cancellationToken);
        await context.LockoutState.AddAsync(lockoutState, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> UpdateAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> ChangeUsernameAsync(UserEntity user, string userName,
        CancellationToken cancellationToken = default)
    {
        user.UserName = userName;
        user.NormalizedUserName = userName.ToUpper();
        user.UserNameChangeDate = DateTimeOffset.UtcNow;
        user.UpdateDate = DateTimeOffset.UtcNow;
        
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> DeleteAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        context.Users.Remove(user);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<bool> CheckPasswordAsync(UserEntity user, string password, CancellationToken cancellationToken = default)
    {
        var result = PasswordHasher.VerifyPassword(password, user.PasswordHash);
        return await Task.FromResult(result);
    }

    public async ValueTask<Result> ChangePasswordAsync(UserEntity user, string newPassword, CancellationToken cancellationToken = default)
    {
        var newPasswordHash = PasswordHasher.HashPassword(newPassword);
        
        user.PasswordHash = newPasswordHash;
        user.PasswordChangeDate = DateTimeOffset.UtcNow;
        user.UpdateDate = DateTimeOffset.UtcNow;
        
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
    
    public string GenerateRandomPassword(int length)
    {
        const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!-_";
        var sb = new StringBuilder();
        var random = new Random();

        for (var i = 0; i < length; i++)
        {
            var randomIndex = random.Next(validChars.Length);
            sb.Append(validChars[randomIndex]);
        }

        return "0" + sb.ToString();
    }
}