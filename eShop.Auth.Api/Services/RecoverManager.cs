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
        if (user.RecoveryCodes.Count > 0)
        {
            context.RecoveryCodes.RemoveRange(user.RecoveryCodes);
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
        if (user.RecoveryCodes.Count == 0)
        {
            return Results.BadRequest("Recovery codes not generated or already used.");
        }

        var entity = user.RecoveryCodes.FirstOrDefault(x => hasher.VerifyHash(code, x.CodeHash));

        if (entity is null)
        {
            return Results.BadRequest("Invalid recovery code.");
        }

        context.RecoveryCodes.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}