namespace eSystem.Product.Api.Interfaces;

public interface ISupplierManager
{
    public ValueTask<List<SupplierEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    public ValueTask<SupplierEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public ValueTask<SupplierEntity?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
}