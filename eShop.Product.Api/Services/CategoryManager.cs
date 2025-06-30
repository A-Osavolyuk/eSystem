namespace eShop.Product.Api.Services;

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