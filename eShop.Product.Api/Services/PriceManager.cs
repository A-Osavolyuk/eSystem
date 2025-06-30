namespace eShop.Product.Api.Services;

public class PriceManager(AppDbContext context) : IPriceManager
{
    private readonly AppDbContext context = context;

    public async ValueTask<List<PriceTypeEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        var entities = await context.PriceType.ToListAsync(cancellationToken);
        return entities;
    }
}