using eShop.Domain.Common.Api;
using eShop.Domain.Requests.Api.Product;

namespace eShop.Infrastructure.Services;

public class ProductService(
    IHttpClientService clientService,
    IConfiguration configuration) : IProductService
{
    private readonly IHttpClientService clientService = clientService;
    private readonly IConfiguration configuration = configuration;

    public async ValueTask<Response> CreateProductAsync(CreateProductRequest request) =>
        await clientService.SendAsync(
            new Request(Url: $"{configuration["Configuration:Services:Proxy:Gateway:Uri"]}/api/v1/Products/create-product",
                Methods: HttpMethods.Post, Data: request));

    public async ValueTask<Response> UpdateProductAsync(UpdateProductRequest request) =>
        await clientService.SendAsync(
            new Request(Url: $"{configuration["Configuration:Services:Proxy:Gateway:Uri"]}/api/v1/Products/update-product",
                Methods: HttpMethods.Put, Data: request));

    public async ValueTask<Response> DeleteProductAsync(DeleteProductRequest request) =>
        await clientService.SendAsync(
            new Request(Url: $"{configuration["Configuration:Services:Proxy:Gateway:Uri"]}/api/v1/Products/delete-product",
                Methods: HttpMethods.Delete, Data: request));

    public async ValueTask<Response> GetProductsAsync() => await clientService.SendAsync(
        new Request(Url: $"{configuration["Configuration:Services:Proxy:Gateway:Uri"]}/api/v1/Products/get-products",
            Methods: HttpMethods.Get));

    public async ValueTask<Response> GetProductByNameAsync(string name) => await clientService.SendAsync(
        new Request(
            Url: $"{configuration["Configuration:Services:Proxy:Gateway:Uri"]}/api/v1/Products/get-product-by-name/{name}",
            Methods: HttpMethods.Get));

    public async ValueTask<Response> GetProductByArticleAsync(string article) => await clientService.SendAsync(
        new Request(
            Url:
            $"{configuration["Configuration:Services:Proxy:Gateway:Uri"]}/api/v1/Products/get-product-by-article/{article}",
            Methods: HttpMethods.Get));

    public async ValueTask<Response> GetProductByIdAsync(Guid id) => await clientService.SendAsync(
        new Request(
            Url: $"{configuration["Configuration:Services:Proxy:Gateway:Uri"]}/api/v1/Products/get-product-by-id/{id}",
            Methods: HttpMethods.Get));
}