using eShop.Auth.Api.Security.Hashing;

namespace eShop.Auth.Api.Services;

[Injectable(typeof(IRecoverManager), ServiceLifetime.Scoped)]
public class RecoverManager(
    AuthDbContext context,
    Hasher hasher) : IRecoverManager
{
    private readonly AuthDbContext context = context;
    private readonly Hasher hasher = hasher;

    public async ValueTask<List<string>> GenerateAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        var existingEntities = await context.RecoveryCodes
            .Where(x => x.UserId == user.Id)
            .ToListAsync(cancellationToken);

        if (existingEntities.Count > 0)
        {
            context.RecoveryCodes.RemoveRange(existingEntities);
            await context.SaveChangesAsync(cancellationToken);
        }
        
        var entities = new List<RecoveryCodeEntity>();
        var codes = new List<string>();
        var rnd = new Random();

        for (var i = 0; i < 10; i++)
        {
            var code = rnd.Next(0, 999_999).ToString().PadLeft(6, '0');
            var hash = hasher.Hash(code);

            codes.Add(code);
            
            var entity = new RecoveryCodeEntity()
            {
                Id = Guid.CreateVersion7(),
                UserId = user.Id,
                Hash = hash,
                CreateDate = DateTimeOffset.UtcNow
            };
            
            entities.Add(entity);
        }
        
        await context.RecoveryCodes.AddRangeAsync(entities, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return codes;
    }

    public async ValueTask<Result> VerifyAsync(UserEntity user, string code, CancellationToken cancellationToken = default)
    {
        var entities = await context.RecoveryCodes
            .Where(x => x.UserId == user.Id)
            .ToListAsync(cancellationToken);

        if (entities.Count == 0)
        {
            return Results.BadRequest("Recovery codes not generated or already used.");
        }

        var entity = entities.FirstOrDefault(x => hasher.VerifyHash(code, x.Hash));

        if (entity is null)
        {
            return Results.BadRequest("Invalid recovery code.");
        }
        
        context.RecoveryCodes.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}