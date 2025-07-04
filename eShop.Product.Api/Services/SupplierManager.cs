namespace eShop.Product.Api.Services;

[Injectable(typeof(ISupplierManager), ServiceLifetime.Scoped)]
public class SupplierManager(AppDbContext context) : ISupplierManager
{
    private readonly AppDbContext context = context;

    public async ValueTask<List<SupplierEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await context.Suppliers.ToListAsync(cancellationToken);
        return entities;
    }

    public async ValueTask<SupplierEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await  context.Suppliers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return entity;
    }

    public async ValueTask<SupplierEntity?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var entity = await  context.Suppliers.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
        return entity;
    }
}