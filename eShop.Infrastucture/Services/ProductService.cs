using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;
using eShop.Domain.Enums;
using eShop.Domain.Options;
using eShop.Domain.Requests.API.Product;

namespace eShop.Infrastructure.Services;

public class ProductService(
    IApiClient c,
    IConfiguration a) : ApiService(a, c), IProductService
{
    public async ValueTask<Response> CreateProductAsync(CreateProductRequest request) => await ApiClient.SendAsync(
        new HttpRequest
            { Url = $"{Gateway}/api/v1/Products/create-product", Method = HttpMethod.Post, Data = request },
        new HttpOptions { ValidateToken = true, WithBearer = true, Type = DataType.Text });

    public async ValueTask<Response> UpdateProductAsync(UpdateProductRequest request) => await ApiClient.SendAsync(
        new HttpRequest
            { Url = $"{Gateway}/api/v1/Products/update-product", Method = HttpMethod.Put, Data = request },
        new HttpOptions { ValidateToken = true, WithBearer = true, Type = DataType.Text });

    public async ValueTask<Response> DeleteProductAsync(DeleteProductRequest request) => await ApiClient.SendAsync(
        new HttpRequest
        {
            Url = $"{Gateway}/api/v1/Products/delete-product", Method = HttpMethod.Delete, Data = request
        },
        new HttpOptions { ValidateToken = true, WithBearer = true, Type = DataType.Text });

    public async ValueTask<Response> GetProductsAsync() => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Products/get-products", Method = HttpMethod.Get },
        new HttpOptions { ValidateToken = true, WithBearer = true, Type = DataType.Text });

    public async ValueTask<Response> GetProductByNameAsync(string name) => await ApiClient.SendAsync(
        new HttpRequest
            { Url = $"{Gateway}/api/v1/Products/get-product-by-name/{name}", Method = HttpMethod.Get },
        new HttpOptions { ValidateToken = true, WithBearer = true, Type = DataType.Text });

    public async ValueTask<Response> GetProductByArticleAsync(string article) => await ApiClient.SendAsync(
        new HttpRequest
            { Url = $"{Gateway}/api/v1/Products/get-product-by-article/{article}", Method = HttpMethod.Get },
        new HttpOptions { ValidateToken = true, WithBearer = true, Type = DataType.Text });

    public async ValueTask<Response> GetProductByIdAsync(Guid id) => await ApiClient.SendAsync(
        new HttpRequest
            { Url = $"{Gateway}/api/v1/Products/get-product-by-id/{id}", Method = HttpMethod.Get },
        new HttpOptions { ValidateToken = true, WithBearer = true, Type = DataType.Text });
}