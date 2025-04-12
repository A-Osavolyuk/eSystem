using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Cart;

namespace eShop.Infrastructure.Services;

internal class CartService(
    IHttpClientService httpClient,
    IConfiguration configuration) : ApiService(configuration, httpClient), ICartService
{
    public async ValueTask<Response> GetCartAsync(Guid userId) => await HttpClientService.SendAsync(
        new Request(
            Url: $"{Configuration[Key]}/api/v1/Carts/get-cart/{userId}",
            Methods: HttpMethods.Get));

    public async ValueTask<Response> UpdateCartAsync(UpdateCartRequest request) => await HttpClientService.SendAsync(
        new Request(
            Url: $"{Configuration[Key]}/api/v1/Carts/update-cart",
            Methods: HttpMethods.Put, Data: request));
}