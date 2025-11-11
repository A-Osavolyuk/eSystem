using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Cryptography.Keys;

namespace eSecurity.Server.Security.Authentication.TwoFactor.Secret;

public sealed class SecretManager(
    AuthDbContext context,
    IKeyFactory keyFactory) : ISecretManager
{
    private readonly AuthDbContext context = context;
    private readonly IKeyFactory keyFactory = keyFactory;

    public async ValueTask<UserSecretEntity?> FindAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        var userSecret = await context.UserSecret.FirstOrDefaultAsync(x => x.UserId == user.Id, cancellationToken);
        return userSecret;
    }

    public async ValueTask<Result> AddAsync(UserSecretEntity secret,
        CancellationToken cancellationToken = default)
    {
        await context.UserSecret.AddAsync(secret, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
    
    public async ValueTask<Result> UpdateAsync(UserSecretEntity secret,
        CancellationToken cancellationToken = default)
    {
        context.UserSecret.Update(secret);
        await context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async ValueTask<Result> RemoveAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        var secret = await context.UserSecret.FirstOrDefaultAsync(
            x => x.UserId == user.Id, cancellationToken);

        if (secret is null)
        {
            return Results.NotFound("Cannot find user secret or doesn't exists");
        }

        context.UserSecret.Remove(secret);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public string Generate() => keyFactory.Create(20);
}