namespace eShop.Product.Api.Interfaces;

public interface ITypeManager
{
    public ValueTask<ProductTypeEntity?> FindAsync(Guid id, CancellationToken cancellationToken);
}