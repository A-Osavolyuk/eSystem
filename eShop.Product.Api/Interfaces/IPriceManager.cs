namespace eShop.Product.Api.Interfaces;

public interface IPriceManager
{
    public ValueTask<List<PriceTypeEntity>> GetAllAsync(CancellationToken cancellationToken);
}