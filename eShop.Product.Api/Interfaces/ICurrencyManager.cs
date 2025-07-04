namespace eShop.Product.Api.Interfaces;

public interface ICurrencyManager
{
    public ValueTask<List<CurrencyEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    public ValueTask<CurrencyEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public ValueTask<CurrencyEntity?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
    public ValueTask<CurrencyEntity?> FindByCodeAsync(string code, CancellationToken cancellationToken = default);
}