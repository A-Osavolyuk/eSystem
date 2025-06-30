namespace eShop.Product.Api.Interfaces;

public interface IUnitManager
{
    public ValueTask<List<UnitEntity>> GetAllAsync(CancellationToken cancellationToken);
    public ValueTask<UnitEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken);
    public ValueTask<UnitEntity?> FindByCodeAsync(string code, CancellationToken cancellationToken);
}