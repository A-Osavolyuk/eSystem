using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Cart;

namespace eShop.Infrastructure.Services;

internal class CartService(
    IHttpClientService httpClient, 
    IConfiguration configuration) : ICartService, IApi
{
    private readonly IHttpClientService httpClient = httpClient;
    private readonly IConfiguration configuration = configuration;

    public async ValueTask<Response> GetCartAsync(Guid userId) => await httpClient.SendAsync(
        new Request(
            Url: $"{configuration["Configuration:Services:Proxy:Gateway:Uri"]}/api/v1/Carts/get-cart/{userId}", Methods: HttpMethods.Get));

    public async ValueTask<Response> UpdateCartAsync(UpdateCartRequest request) => await httpClient.SendAsync(
        new Request(
            Url: $"{configuration["Configuration:Services:Proxy:Gateway:Uri"]}/api/v1/Carts/update-cart",
            Methods: HttpMethods.Put, Data: request));
}