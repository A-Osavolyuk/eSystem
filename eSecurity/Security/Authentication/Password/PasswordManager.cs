using eSecurity.Data.Entities;
using eSecurity.Security.Cryptography.Hashing;

namespace eSecurity.Security.Authentication.Password;

public class PasswordManager(
    AuthDbContext context,
    IHasherFactory hasherFactory) : IPasswordManager
{
    private readonly AuthDbContext context = context;
    private readonly Hasher hasher = hasherFactory.Create(HashAlgorithm.Pbkdf2);

    public async ValueTask<Result> AddAsync(UserEntity user, string password, 
        CancellationToken cancellationToken = default)
    {
        var passwordEntity = new PasswordEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Hash = hasher.Hash(password),
            CreateDate = DateTimeOffset.UtcNow
        };
        
        await context.Passwords.AddAsync(passwordEntity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> ChangeAsync(UserEntity user, string newPassword, 
        CancellationToken cancellationToken = default)
    {
        var passwordEntity = user.Password;
        if (passwordEntity is null) return Results.BadRequest("User doesn't have password yet.");
        
        passwordEntity.Hash = hasher.Hash(newPassword);
        passwordEntity.UpdateDate = DateTimeOffset.UtcNow;
        
        context.Passwords.Update(passwordEntity);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> ResetAsync(UserEntity user, string newPassword, 
        CancellationToken cancellationToken = default)
    {
        var passwordEntity = user.Password;
        if (passwordEntity is null) return Results.BadRequest("User doesn't have password yet.");
        
        passwordEntity.Hash = hasher.Hash(newPassword);
        passwordEntity.UpdateDate = DateTimeOffset.UtcNow;
        
        context.Passwords.Update(passwordEntity);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> RemoveAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        var passwordEntity = user.Password;
        if (passwordEntity is null) return Results.BadRequest("User doesn't have password yet.");
        
        context.Passwords.Remove(passwordEntity);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public bool Check(UserEntity user, string password) 
        => user.Password is not null && hasher.VerifyHash(password, user.Password.Hash);
}