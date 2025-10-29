using eSystem.Auth.Api.Data.Entities;
using eSystem.Core.Attributes;
using eSystem.Core.Security.Cryptography.Keys;
using eSystem.Core.Security.Cryptography.Protection;

namespace eSystem.Auth.Api.Security.Authentication.TwoFactor.Secret;

[Injectable(typeof(ISecretManager), ServiceLifetime.Scoped)]
public sealed class SecretManager(
    AuthDbContext context,
    IProtectorFactory protectorFactory,
    IKeyFactory keyFactory) : ISecretManager
{
    private readonly AuthDbContext context = context;
    private readonly IKeyFactory keyFactory = keyFactory;
    private readonly IProtector protector = protectorFactory.Create(ProtectionPurposes.Secret);

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
    public string Protect(string unprotectedSecret) => protector.Protect(unprotectedSecret);
    public string Unprotect(string protectedSecret) => protector.Unprotect(protectedSecret);
}