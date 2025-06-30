namespace eShop.Product.Api.Interfaces;

public interface ITypeManager
{
    public ValueTask<List<TypeEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    public ValueTask<TypeEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken);
}