namespace eShop.Product.Api.Interfaces;

public interface ITypeManager
{
    public ValueTask<List<ProductTypeEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    public ValueTask<ProductTypeEntity?> FindAsync(Guid id, CancellationToken cancellationToken);
}