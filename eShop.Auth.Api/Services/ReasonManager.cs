namespace eShop.Auth.Api.Services;

[Injectable(typeof(IReasonManager),  ServiceLifetime.Scoped)]
public class ReasonManager(AuthDbContext context) : IReasonManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<List<LockoutReasonEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await context.LockoutReasons.ToListAsync(cancellationToken);
        return entities;
    }

    public async ValueTask<LockoutReasonEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await context.LockoutReasons.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return entity;
    }

    public async ValueTask<LockoutReasonEntity?> FindByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var entity = await context.LockoutReasons.FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
        return entity;
    }

    public async ValueTask<LockoutReasonEntity?> FindByTypeAsync(LockoutType type, CancellationToken cancellationToken = default)
    {
        var entity = await context.LockoutReasons.FirstOrDefaultAsync(x => x.Type == type, cancellationToken);
        return entity;
    }
}