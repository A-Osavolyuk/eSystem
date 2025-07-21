namespace eShop.Auth.Api.Services;

[Injectable(typeof(IRecoverManager), ServiceLifetime.Scoped)]
public class RecoverManager(AuthDbContext context) : IRecoverManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<List<RecoveryCodeEntity>> FindAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        var entities = await context.RecoveryCodes
            .Where(x => x.UserId == user.Id)
            .ToListAsync(cancellationToken);

        return entities;
    }

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
        var rnd = new Random();

        for (var i = 0; i < 10; i++)
        {
            var code = rnd.Next(0, 999_999).ToString().PadLeft(6, '0');

            var entity = new RecoveryCodeEntity()
            {
                Id = Guid.CreateVersion7(),
                UserId = user.Id,
                Code = code,
                CreateDate = DateTimeOffset.UtcNow
            };
            
            entities.Add(entity);
        }
        
        await context.RecoveryCodes.AddRangeAsync(entities, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        return entities.Select(entity => entity.Code).ToList();
    }

    public async ValueTask<Result> VerifyAsync(UserEntity user, string code, CancellationToken cancellationToken = default)
    {
        var entity = await context.RecoveryCodes.FirstOrDefaultAsync(
            x => x.UserId == user.Id && x.Code == code, cancellationToken);

        if (entity is null)
        {
            return Results.BadRequest("Recovery code not exists or already used.");
        }
        
        context.RecoveryCodes.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}