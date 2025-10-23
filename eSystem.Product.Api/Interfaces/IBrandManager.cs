using eSystem.Product.Api.Entities;

namespace eSystem.Product.Api.Interfaces;

public interface IBrandManager
{
    public ValueTask<List<BrandEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    public ValueTask<BrandEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public ValueTask<BrandEntity?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
}