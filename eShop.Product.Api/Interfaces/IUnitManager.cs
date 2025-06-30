namespace eShop.Product.Api.Interfaces;

public interface IUnitManager
{
    public ValueTask<List<UnitEntity>> GetAllAsync(CancellationToken cancellationToken);
}