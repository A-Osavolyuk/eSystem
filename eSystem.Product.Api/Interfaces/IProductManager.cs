using eSystem.Domain.Common.Results;
using eSystem.Product.Api.Entities;

namespace eSystem.Product.Api.Interfaces;

public interface IProductManager
{
    public ValueTask<Result> CreateAsync(ProductEntity entity, CancellationToken cancellationToken = default);
    public ValueTask<Result> UpdateAsync(ProductEntity entity, CancellationToken cancellationToken = default);
    public ValueTask<Result> DeleteAsync(ProductEntity entity, CancellationToken cancellationToken = default);
}