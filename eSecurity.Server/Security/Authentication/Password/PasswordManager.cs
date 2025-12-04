using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Cryptography.Hashing;

namespace eSecurity.Server.Security.Authentication.Password;

public class PasswordManager(
    AuthDbContext context,
    IHasherFactory hasherFactory) : IPasswordManager
{
    private readonly AuthDbContext _context = context;
    private readonly IHasher _hasher = hasherFactory.CreateHasher(HashAlgorithm.Pbkdf2);

    public async ValueTask<Result> AddAsync(UserEntity user, string password, 
        CancellationToken cancellationToken = default)
    {
        var passwordEntity = new PasswordEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Hash = _hasher.Hash(password),
            CreateDate = DateTimeOffset.UtcNow
        };
        
        await _context.Passwords.AddAsync(passwordEntity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Ok();
    }

    public async ValueTask<Result> ChangeAsync(UserEntity user, string newPassword, 
        CancellationToken cancellationToken = default)
    {
        var passwordEntity = user.Password;
        if (passwordEntity is null) return Results.BadRequest("User doesn't have password yet.");
        
        passwordEntity.Hash = _hasher.Hash(newPassword);
        passwordEntity.UpdateDate = DateTimeOffset.UtcNow;
        
        _context.Passwords.Update(passwordEntity);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Ok();
    }

    public async ValueTask<Result> ResetAsync(UserEntity user, string newPassword, 
        CancellationToken cancellationToken = default)
    {
        var passwordEntity = user.Password;
        if (passwordEntity is null) return Results.BadRequest("User doesn't have password yet.");
        
        passwordEntity.Hash = _hasher.Hash(newPassword);
        passwordEntity.UpdateDate = DateTimeOffset.UtcNow;
        
        _context.Passwords.Update(passwordEntity);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Ok();
    }

    public async ValueTask<Result> RemoveAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        var passwordEntity = user.Password;
        if (passwordEntity is null) return Results.BadRequest("User doesn't have password yet.");
        
        _context.Passwords.Remove(passwordEntity);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Ok();
    }

    public bool Check(UserEntity user, string password) 
        => user.Password is not null && _hasher.VerifyHash(password, user.Password.Hash);
}