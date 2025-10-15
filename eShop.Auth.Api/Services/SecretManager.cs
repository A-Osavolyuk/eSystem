using eShop.Auth.Api.Security.Protection;
using OtpNet;

namespace eShop.Auth.Api.Services;

[Injectable(typeof(ISecretManager), ServiceLifetime.Scoped)]
public sealed class SecretManager(
    AuthDbContext context,
    SecretProtector protector) : ISecretManager
{
    private readonly AuthDbContext context = context;
    private readonly SecretProtector protector = protector;

    public async ValueTask<UserSecretEntity?> FindAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        var userSecret = await context.UserSecret.FirstOrDefaultAsync(x => x.UserId == user.Id, cancellationToken);
        return userSecret;
    }

    public async ValueTask<UserSecretEntity> GenerateAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        var secretKey = KeyGenerator.GenerateKey(20);
        var protectedSecret = protector.Protect(secretKey);
        
        var entity = new UserSecretEntity()
        {
            Id = Guid.CreateVersion7(),
            Secret = protectedSecret,
            UserId = user.Id,
            CreateDate = DateTime.UtcNow,
            UpdateDate = null
        };
        
        await context.UserSecret.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        return entity;
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
}