using eShop.Auth.Api.Security.Protection;
using OtpNet;

namespace eShop.Auth.Api.Services;

[Injectable(typeof(ISecretManager), ServiceLifetime.Scoped)]
public sealed class SecretManager(
    AuthDbContext context,
    IProtectorFactory protectorFactory) : ISecretManager
{
    private readonly AuthDbContext context = context;
    private readonly Protector protector = protectorFactory.Create(ProtectorType.Secret);

    public async ValueTask<UserSecretEntity?> FindAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        var userSecret = await context.UserSecret.FirstOrDefaultAsync(x => x.UserId == user.Id, cancellationToken);
        return userSecret;
    }

    public async ValueTask<Result> SaveAsync(UserSecretEntity secret,
        CancellationToken cancellationToken = default)
    {
        await context.UserSecret.AddAsync(secret, cancellationToken);
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

    public string Generate() => KeyGenerator.GenerateKey(20);
    public string Protect(string unprotectedSecret) => protector.Protect(unprotectedSecret);
    public string Unprotect(string protectedSecret) => protector.Protect(protectedSecret);
}