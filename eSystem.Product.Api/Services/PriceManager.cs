using eSystem.Core.Attributes;
using eSystem.Product.Api.Data;
using eSystem.Product.Api.Entities;
using eSystem.Product.Api.Interfaces;

namespace eSystem.Product.Api.Services;

[Injectable(typeof(IPriceManager), ServiceLifetime.Scoped)]
public class PriceManager(AppDbContext context) : IPriceManager
{
    private readonly AppDbContext context = context;

    public async ValueTask<List<PriceTypeEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        var entities = await context.PriceType.ToListAsync(cancellationToken);
        return entities;
    }

    public async ValueTask<PriceTypeEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await context.PriceType.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return entity;
    }
}