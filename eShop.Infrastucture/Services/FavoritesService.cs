using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Favorites;

namespace eShop.Infrastructure.Services;

public class FavoritesService(
    IHttpClientService httpClient,
    IConfiguration configuration) : Api(configuration, httpClient), IFavoritesService
{
    public async ValueTask<Response> GetFavoritesAsync(Guid userId) => await HttpClientService.SendAsync(
        new Request(
            Url: $"{Configuration[Key]}/api/v1/Favorites/get-favorites/{userId}",
            Methods: HttpMethods.Get));

    public async ValueTask<Response> UpdateFavoritesAsync(UpdateFavoritesRequest request) =>
        await HttpClientService.SendAsync(
            new Request(
                Url: $"{Configuration[Key]}/api/v1/Favorites/update-favorites",
                Methods: HttpMethods.Put, Data: request));
}