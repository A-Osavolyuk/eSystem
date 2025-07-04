namespace eShop.Product.Api.Services;

[Injectable(typeof(IUnitManager), ServiceLifetime.Scoped)]
public class UnitManager(AppDbContext context) : IUnitManager
{
    private readonly AppDbContext context = context;

    public async ValueTask<List<UnitEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        var entities = await context.Units.ToListAsync(cancellationToken);
        return entities;
    }

    public async ValueTask<UnitEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await context.Units.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return entity;
    }

    public async ValueTask<UnitEntity?> FindByCodeAsync(string code, CancellationToken cancellationToken)
    {
        var entity = await context.Units.FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
        return entity;
    }
}