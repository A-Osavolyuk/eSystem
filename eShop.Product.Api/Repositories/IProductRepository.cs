using eShop.Domain.DTOs;
using eShop.Domain.Requests.Api.Product;

namespace eShop.Product.Api.Repositories;

public interface IProductRepository
{
    public ValueTask<IEnumerable<ProductDto>> GetProductsAsync();
    public ValueTask<IEnumerable<ProductDto>> GetProductsByTypeAsync(ProductTypes productType);
    public ValueTask<IEnumerable<ProductDto>> FindProductsByNameAsync(string name);
    public ValueTask<ProductDto?> FindProductByIdAsync(Guid productId);
    public ValueTask<ProductDto?> FindProductByArticleAsync(string article);
    public ValueTask<ProductDto?> FindProductByNameAsync(string name);
    public ValueTask CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken = default);
    public ValueTask UpdateProductAsync(UpdateProductRequest request, CancellationToken cancellationToken = default);

    public ValueTask<bool> DeleteProductAsync(DeleteProductRequest request,
        CancellationToken cancellationToken = default);
}