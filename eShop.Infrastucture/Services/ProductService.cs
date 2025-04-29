using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Product;

namespace eShop.Infrastructure.Services;

public class ProductService(
    IHttpClientService c,
    IConfiguration a) : ApiService(a, c), IProductService
{
    public async ValueTask<Response> CreateProductAsync(CreateProductRequest request) =>
        await HttpClientService.SendAsync(
            new Request(Url: $"{Configuration[Key]}/api/v1/Products/create-product",
                Method: HttpMethod.Post, Data: request));

    public async ValueTask<Response> UpdateProductAsync(UpdateProductRequest request) =>
        await HttpClientService.SendAsync(
            new Request(Url: $"{Configuration[Key]}/api/v1/Products/update-product",
                Method: HttpMethod.Put, Data: request));

    public async ValueTask<Response> DeleteProductAsync(DeleteProductRequest request) =>
        await HttpClientService.SendAsync(
            new Request(Url: $"{Configuration[Key]}/api/v1/Products/delete-product",
                Method: HttpMethod.Delete, Data: request));

    public async ValueTask<Response> GetProductsAsync() => await HttpClientService.SendAsync(
        new Request(Url: $"{Configuration[Key]}/api/v1/Products/get-products",
            Method: HttpMethod.Get));

    public async ValueTask<Response> GetProductByNameAsync(string name) => await HttpClientService.SendAsync(
        new Request(
            Url: $"{Configuration[Key]}/api/v1/Products/get-product-by-name/{name}",
            Method: HttpMethod.Get));

    public async ValueTask<Response> GetProductByArticleAsync(string article) => await HttpClientService.SendAsync(
        new Request(
            Url:
            $"{Configuration[Key]}/api/v1/Products/get-product-by-article/{article}",
            Method: HttpMethod.Get));

    public async ValueTask<Response> GetProductByIdAsync(Guid id) => await HttpClientService.SendAsync(
        new Request(
            Url: $"{Configuration[Key]}/api/v1/Products/get-product-by-id/{id}",
            Method: HttpMethod.Get));
}