using eSystem.Product.Api.Data;
using eSystem.Product.Api.Entities;
using eSystem.Product.Api.Interfaces;

namespace eSystem.Product.Api.Services;

[Injectable(typeof(ICategoryManager), ServiceLifetime.Scoped)]
public class CategoryManager(AppDbContext context) : ICategoryManager
{
    private readonly AppDbContext context = context;

    public async ValueTask<List<CategoryEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await context.Categories.ToListAsync(cancellationToken);
        return entities;
    }

    public async ValueTask<CategoryEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        return entity;
    }
}