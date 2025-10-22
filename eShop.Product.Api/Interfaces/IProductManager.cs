using eShop.Domain.Common.Results;

namespace eShop.Product.Api.Interfaces;

public interface IProductManager
{
    public ValueTask<Result> CreateAsync(ProductEntity entity, CancellationToken cancellationToken = default);
    public ValueTask<Result> UpdateAsync(ProductEntity entity, CancellationToken cancellationToken = default);
    public ValueTask<Result> DeleteAsync(ProductEntity entity, CancellationToken cancellationToken = default);
}