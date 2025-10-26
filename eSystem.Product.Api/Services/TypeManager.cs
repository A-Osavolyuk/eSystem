using eSystem.Core.Attributes;
using eSystem.Product.Api.Data;
using eSystem.Product.Api.Entities;
using eSystem.Product.Api.Interfaces;

namespace eSystem.Product.Api.Services;

[Injectable(typeof(ITypeManager), ServiceLifetime.Scoped)]
public class TypeManager(AppDbContext context) : ITypeManager
{
    private readonly AppDbContext context = context;

    public async ValueTask<List<TypeEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await context.Types
            .Include(x => x.Category)
            .ToListAsync(cancellationToken);
        
        return entities.ToList();
    }

    public async ValueTask<TypeEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await context.Types
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        
        return entity;
    }
}