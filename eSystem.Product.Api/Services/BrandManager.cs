using eSystem.Product.Api.Data;
using eSystem.Product.Api.Entities;
using eSystem.Product.Api.Interfaces;

namespace eSystem.Product.Api.Services;

[Injectable(typeof(IBrandManager), ServiceLifetime.Scoped)]
public class BrandManager(AppDbContext context) : IBrandManager
{
    private readonly AppDbContext context = context;

    public async ValueTask<List<BrandEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await context.Brands.ToListAsync(cancellationToken);
        return entities;
    }

    public async ValueTask<BrandEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Brands.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return entity;
    }

    public async ValueTask<BrandEntity?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var entity = await context.Brands.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
        return entity;
    }
}