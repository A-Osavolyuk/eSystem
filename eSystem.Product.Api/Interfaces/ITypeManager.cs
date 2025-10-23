using eSystem.Product.Api.Entities;

namespace eSystem.Product.Api.Interfaces;

public interface ITypeManager
{
    public ValueTask<List<TypeEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    public ValueTask<TypeEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken);
}