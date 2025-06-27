namespace eShop.Product.Api.Interfaces;

public interface IProductManager
{
    public ValueTask<Result> CreateAsync(ProductEntity entity);
    public ValueTask<Result> UpdateAsync(ProductEntity entity);
    public ValueTask<Result> DeleteAsync(ProductEntity entity);
}