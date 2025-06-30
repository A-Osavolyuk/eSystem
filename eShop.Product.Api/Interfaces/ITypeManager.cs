namespace eShop.Product.Api.Interfaces;

public interface ITypeManager
{
    public ValueTask<List<TypeEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    public ValueTask<TypeEntity?> FindAsync(Guid id, CancellationToken cancellationToken);
}