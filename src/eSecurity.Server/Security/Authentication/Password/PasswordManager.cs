using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Cryptography.Hashing;

namespace eSecurity.Server.Security.Authentication.Password;

public class PasswordManager(
    AuthDbContext context,
    IHasherProvider hasherProvider) : IPasswordManager
{
    private readonly AuthDbContext _context = context;
    private readonly IHasher _hasher = hasherProvider.GetHasher(HashAlgorithm.Pbkdf2);

    public async ValueTask<PasswordEntity?> GetAsync(UserEntity user, CancellationToken cancellationToken = default)
        => await _context.Passwords.FirstOrDefaultAsync(x => x.UserId == user.Id, cancellationToken);

    public async ValueTask<Result> AddAsync(UserEntity user, string password,
        CancellationToken cancellationToken = default)
    {
        var passwordEntity = new PasswordEntity
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Hash = _hasher.Hash(password)
        };

        await _context.Passwords.AddAsync(passwordEntity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> ChangeAsync(UserEntity user, string newPassword,
        CancellationToken cancellationToken = default)
    {
        var passwordEntity = await GetAsync(user, cancellationToken);
        if (passwordEntity is null) return Results.BadRequest("User doesn't have password yet.");

        passwordEntity.Hash = _hasher.Hash(newPassword);

        _context.Passwords.Update(passwordEntity);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> ResetAsync(UserEntity user, string newPassword,
        CancellationToken cancellationToken = default)
    {
        var passwordEntity = await GetAsync(user, cancellationToken);
        if (passwordEntity is null) return Results.BadRequest("User doesn't have password yet.");

        passwordEntity.Hash = _hasher.Hash(newPassword);

        _context.Passwords.Update(passwordEntity);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> RemoveAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        var passwordEntity = await GetAsync(user, cancellationToken);
        if (passwordEntity is null) return Results.BadRequest("User doesn't have password yet.");

        _context.Passwords.Remove(passwordEntity);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<bool> HasAsync(UserEntity user, CancellationToken cancellationToken = default)
        => await _context.Passwords.AnyAsync(x => x.UserId == user.Id, cancellationToken);

    public async ValueTask<bool> CheckAsync(UserEntity user, string password,
        CancellationToken cancellationToken = default)
    {
        var entity = await GetAsync(user, cancellationToken);
        return entity is not null && _hasher.VerifyHash(password, entity.Hash);
    }
}