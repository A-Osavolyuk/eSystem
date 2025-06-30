namespace eShop.Product.Api.Interfaces;

public interface ICategoryManager
{
    public ValueTask<List<CategoryEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    public ValueTask<CategoryEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
}