namespace eShop.Product.Api.Services;

public class CurrencyManager(AppDbContext context) : ICurrencyManager
{
    private readonly AppDbContext context = context;

    public async ValueTask<List<CurrencyEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await context.Currency.ToListAsync(cancellationToken);
        return entities;
    }

    public async ValueTask<CurrencyEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Currency.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return entity;
    }

    public async ValueTask<CurrencyEntity?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var entity = await context.Currency.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
        return entity;
    }

    public async ValueTask<CurrencyEntity?> FindByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var entity = await context.Currency.FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
        return entity;
    }
}