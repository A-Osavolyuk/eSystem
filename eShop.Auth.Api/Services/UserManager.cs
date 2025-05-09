namespace eShop.Auth.Api.Services;

public class UserManager(AuthDbContext context) : IUserManager
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

    public async ValueTask<bool> IsInRoleAsync(UserEntity user, string roleName,
        CancellationToken cancellationToken = default)
    {
        var isInRole = await context.UserRoles
            .AnyAsync(x => x.UserId == user.Id && x.Role.Name == roleName, cancellationToken: cancellationToken);

        return isInRole;
    }

    public async ValueTask<Result> ConfirmEmailAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        user.EmailConfirmed = true;
        user.UpdateDate = DateTime.UtcNow;
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> ConfirmPhoneNumberAsync(UserEntity user,
        CancellationToken cancellationToken = default)
    {
        user.PhoneNumberConfirmed = true;
        user.UpdateDate = DateTime.UtcNow;
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> ResetPasswordAsync(UserEntity user, string newPassword,
        CancellationToken cancellationToken = default)
    {
        var passwordHash = PasswordHasher.HashPassword(newPassword);

        user.PasswordHash = passwordHash;
        user.UpdateDate = DateTime.UtcNow;
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> ChangeEmailAsync(UserEntity user, string newEmail,
        CancellationToken cancellationToken = default)
    {
        user.Email = newEmail;
        user.NormalizedEmail = newEmail.ToUpperInvariant();
        user.UpdateDate = DateTime.UtcNow;
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> ChangePhoneNumberAsync(UserEntity user, string newPhoneNumber,
        CancellationToken cancellationToken = default)
    {
        user.PhoneNumber = newPhoneNumber;
        user.UpdateDate = DateTime.UtcNow;
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> CreateAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        user.NormalizedEmail = user.Email.ToUpper();
        user.NormalizedUserName = user.UserName.ToUpper();
        user.CreateDate = DateTime.UtcNow;
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> CreateAsync(UserEntity user, string password,
        CancellationToken cancellationToken = default)
    {
        var passwordHash = PasswordHasher.HashPassword(password);

        user.PasswordHash = passwordHash;
        user.NormalizedEmail = user.Email.ToUpper();
        user.NormalizedUserName = user.UserName.ToUpper();
        user.CreateDate = DateTime.UtcNow;
        context.Users.Update(user);
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

    public async ValueTask<Result> AddToRoleAsync(UserEntity user, string roleName,
        CancellationToken cancellationToken = default)
    {
        var role = await context.Roles.FirstOrDefaultAsync(x => x.Name == roleName || x.NormalizedName == roleName,
            cancellationToken);

        if (role is null)
        {
            return Results.NotFound("Role not found");
        }

        var userRole = new UserRoleEntity()
        {
            UserId = user.Id,
            RoleId = role.Id,
            CreateDate = DateTime.UtcNow
        };

        await context.UserRoles.AddAsync(userRole, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> AddToRoleAsync(UserEntity user, RoleEntity role,
        CancellationToken cancellationToken = default)
    {
        var userRole = new UserRoleEntity()
        {
            UserId = user.Id,
            RoleId = role.Id,
            CreateDate = DateTime.UtcNow
        };

        await context.UserRoles.AddAsync(userRole, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> AddPasswordAsync(UserEntity user, string password,
        CancellationToken cancellationToken = default)
    {
        var passwordHash = PasswordHasher.HashPassword(password);

        user.PasswordHash = passwordHash;
        user.UpdateDate = DateTime.UtcNow;
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> RemoveFromRoleAsync(UserEntity user, string roleName,
        CancellationToken cancellationToken = default)
    {
        var role = await context.UserRoles
            .FirstOrDefaultAsync(x => x.UserId == user.Id
                                      && (x.Role.Name == roleName || x.Role.NormalizedName == roleName),
                cancellationToken);

        if (role is null)
        {
            return Results.NotFound("User not in role");
        }

        context.UserRoles.Remove(role);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> RemoveFromRoleAsync(UserEntity user, RoleEntity role,
        CancellationToken cancellationToken = default)
    {
        var userRole = await context.UserRoles
            .FirstOrDefaultAsync(x => x.UserId == user.Id && x.RoleId == role.Id, cancellationToken);
        
        if (userRole is null)
        {
            return Results.NotFound("User not in role");
        }
        
        context.UserRoles.Remove(userRole);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> RemoveFromRolesAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        var userRoles = await context.UserRoles
            .Where(x => x.UserId == user.Id)
            .ToListAsync(cancellationToken);
        
        context.UserRoles.RemoveRange(userRoles);
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
}