using eSystem.Product.Api.Entities;

namespace eSystem.Product.Api.Interfaces;

public interface IPriceManager
{
    public ValueTask<List<PriceTypeEntity>> GetAllAsync(CancellationToken cancellationToken);
    public ValueTask<PriceTypeEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
}