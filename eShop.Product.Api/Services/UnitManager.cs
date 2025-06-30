namespace eShop.Product.Api.Services;

public class UnitManager(AppDbContext context) : IUnitManager
{
    private readonly AppDbContext context = context;

    public async ValueTask<List<UnitEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        var entities = await context.Units.ToListAsync(cancellationToken);
        return entities;
    }
}