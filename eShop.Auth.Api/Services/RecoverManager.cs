using eShop.Auth.Api.Security.Hashing;

namespace eShop.Auth.Api.Services;

[Injectable(typeof(IRecoverManager), ServiceLifetime.Scoped)]
public sealed class RecoverManager(
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

        var codes = CodeGenerator.GenerateMany(6);

        var entities = codes
            .Select(code => hasher.Hash(code))
            .Select(hash => new RecoveryCodeEntity()
            {
                Id = Guid.CreateVersion7(),
                UserId = user.Id,
                CodeHash = hash,
                CreateDate = DateTimeOffset.UtcNow
            })
            .ToList();

        await context.RecoveryCodes.AddRangeAsync(entities, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return codes;
    }

    public async ValueTask<Result> VerifyAsync(UserEntity user, string code,
        CancellationToken cancellationToken = default)
    {
        var entities = await context.RecoveryCodes
            .Where(x => x.UserId == user.Id)
            .ToListAsync(cancellationToken);

        if (entities.Count == 0)
        {
            return Results.BadRequest("Recovery codes not generated or already used.");
        }

        var entity = entities.FirstOrDefault(x => hasher.VerifyHash(code, x.CodeHash));

        if (entity is null)
        {
            return Results.BadRequest("Invalid recovery code.");
        }

        context.RecoveryCodes.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}