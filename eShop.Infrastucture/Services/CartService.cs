using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;
using eShop.Domain.Options;
using eShop.Domain.Requests.API.Cart;

namespace eShop.Infrastructure.Services;

internal class CartService(
    IApiClient httpClient,
    IConfiguration configuration) : ApiService(configuration, httpClient), ICartService
{
    public async ValueTask<Response> GetCartAsync(Guid userId) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Carts/get-cart/{userId}", Method = HttpMethod.Get },
        new HttpOptions { ValidateToken = true, WithBearer = true });

    public async ValueTask<Response> UpdateCartAsync(UpdateCartRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Carts/update-cart", Method = HttpMethod.Put, Data = request },
        new HttpOptions { ValidateToken = true, WithBearer = true });


}