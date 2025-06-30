using eShop.Product.Api.Interfaces;

namespace eShop.Product.Api.Services;

public class TypeManager(AppDbContext context) : ITypeManager
{
    private readonly AppDbContext context = context;

    public async ValueTask<ProductTypeEntity?> FindAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await context.Types.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return entity;
    }
}