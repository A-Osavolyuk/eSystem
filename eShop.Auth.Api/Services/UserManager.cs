namespace eShop.Auth.Api.Services;

[Injectable(typeof(IUserManager), ServiceLifetime.Scoped)]
public sealed class UserManager(
    AuthDbContext context,
    ICodeManager codeManager) : IUserManager
{
    private readonly AuthDbContext context = context;
    private readonly ICodeManager codeManager = codeManager;

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

    public async ValueTask<Result> ConfirmEmailAsync(UserEntity user, string code, CancellationToken cancellationToken = default)
    {
        var result = await codeManager.VerifyAsync(user, code, CodeType.Verify, true, cancellationToken);

        if (!result.Succeeded)
        {
            return result;
        }
        
        user.EmailConfirmed = true;
        user.UpdateDate = DateTime.UtcNow;
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> ConfirmPhoneNumberAsync(UserEntity user, string code,
        CancellationToken cancellationToken = default)
    {
        var result = await codeManager.VerifyAsync(user, code, CodeType.Verify, true, cancellationToken);

        if (!result.Succeeded)
        {
            return result;
        }
        
        user.PhoneNumberConfirmed = true;
        user.UpdateDate = DateTime.UtcNow;
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> ResetPasswordAsync(UserEntity user, string code, string newPassword,
        CancellationToken cancellationToken = default)
    {
        var result = await codeManager.VerifyAsync(user, code, CodeType.Reset, true, cancellationToken);

        if (!result.Succeeded)
        {
            return result;
        }
        
        var passwordHash = PasswordHasher.HashPassword(newPassword);

        user.PasswordHash = passwordHash;
        user.UpdateDate = DateTime.UtcNow;
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> ChangeEmailAsync(UserEntity user, string newEmail, string currentEmailCode, 
        string newEmailCode, CancellationToken cancellationToken = default)
    {
        var currentEmailResult = await codeManager.VerifyAsync(user, currentEmailCode, CodeType.Current, true, cancellationToken);

        if (!currentEmailResult.Succeeded)
        {
            return currentEmailResult;
        }
        
        var newEmailResult = await codeManager.VerifyAsync(user, newEmailCode, CodeType.New, true, cancellationToken);
        
        if (!newEmailResult.Succeeded)
        {
            return newEmailResult;
        }
        
        user.Email = newEmail;
        user.NormalizedEmail = newEmail.ToUpperInvariant();
        user.UpdateDate = DateTime.UtcNow;
        user.UserName = newEmail;
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> ChangePhoneNumberAsync(UserEntity user, string currentPhoneNumberCode,
        string newPhoneNumberCode, string newPhoneNumber, CancellationToken cancellationToken = default)
    {
        var currentPhoneNumberResult = await codeManager.VerifyAsync(user, currentPhoneNumberCode, CodeType.Current, true, cancellationToken);

        if (!currentPhoneNumberResult.Succeeded)
        {
            return currentPhoneNumberResult;
        }
        
        var newPhoneNumberResult = await codeManager.VerifyAsync(user, newPhoneNumberCode, CodeType.New, true, cancellationToken);

        if (!newPhoneNumberResult.Succeeded)
        {
            return newPhoneNumberResult;
        }
        
        user.PhoneNumber = newPhoneNumber;
        user.UpdateDate = DateTime.UtcNow;
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
            CreateDate = DateTime.UtcNow,
        };
        
        var passwordHash = PasswordHasher.HashPassword(password);

        user.PasswordHash = passwordHash;
        user.NormalizedEmail = user.Email.ToUpper();
        user.NormalizedUserName = user.UserName.ToUpper();
        user.CreateDate = DateTime.UtcNow;
        
        await context.Users.AddAsync(user, cancellationToken);
        await context.LockoutState.AddAsync(lockoutState, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> SetUserNameAsync(UserEntity user, string userName,
        CancellationToken cancellationToken = default)
    {
        user.UserName = userName;
        user.NormalizedUserName = userName.ToUpper();
        user.UpdateDate = DateTime.UtcNow;
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

    public async ValueTask<Result> ChangePasswordAsync(UserEntity user, string currentPassword, string newPassword,
        CancellationToken cancellationToken = default)
    {
        if (PasswordHasher.VerifyPassword(currentPassword, user.PasswordHash))
        {
            return Results.BadRequest("Incorrect password");
        }
        
        var newPasswordHash = PasswordHasher.HashPassword(newPassword);
        
        user.PasswordHash = newPasswordHash;
        user.UpdateDate = DateTime.UtcNow;
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