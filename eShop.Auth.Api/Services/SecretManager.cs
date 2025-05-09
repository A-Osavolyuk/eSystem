using OtpNet;

namespace eShop.Auth.Api.Services;

public class SecretManager(AuthDbContext context) : ISecretManager
{
    private readonly AuthDbContext context = context;
    
    public async ValueTask<UserSecretEntity?> FindAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        var userSecret = await context.UserSecret.FirstOrDefaultAsync(x => x.UserId == user.Id, cancellationToken);
        return userSecret;
    }

    public async ValueTask<string> GenerateAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        var secretKey = KeyGeneration.GenerateRandomKey(20);
        var base32Secret = Base32Encoding.ToString(secretKey);

        var entity = new UserSecretEntity()
        {
            Id = Guid.CreateVersion7(),
            Secret = base32Secret,
            UserId = user.Id,
            CreateDate = DateTime.UtcNow,
            UpdateDate = null
        };
        
        await context.UserSecret.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        return base32Secret;
    }

    public async ValueTask<Result> DeleteAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        var userSecret = await context.UserSecret.FirstOrDefaultAsync(x => x.UserId == user.Id, cancellationToken);
        
        if (userSecret is null)
        {
            return Results.NotFound("Not found user secret");
        }
        
        context.UserSecret.Remove(userSecret);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}