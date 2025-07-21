namespace eShop.Auth.Api.Services;

[Injectable(typeof(IRecoverManager), ServiceLifetime.Scoped)]
public class RecoverManager(AuthDbContext context) : IRecoverManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<List<string>> GenerateAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        var entities = new List<RecoveryCodeEntity>();
        var rnd = new Random();

        for (var i = 0; i < 10; i++)
        {
            var code = rnd.Next(0, 10_000_000).ToString().PadLeft(8, '0');

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
}