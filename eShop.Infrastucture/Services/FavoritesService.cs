using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;
using eShop.Domain.Options;
using eShop.Domain.Requests.API.Favorites;

namespace eShop.Infrastructure.Services;

public class FavoritesService(
    IApiClient httpClient,
    IConfiguration configuration) : ApiService(configuration, httpClient), IFavoritesService
{
    public async ValueTask<Response> GetFavoritesAsync(Guid userId) => await ApiClient.SendAsync(
        new HttpRequest(
            Url: $"{Configuration[Key]}/api/v1/Favorites/get-favorites/{userId}",
            Method: HttpMethod.Get),
        new HttpOptions() { ValidateToken = true, WithBearer = true });

    public async ValueTask<Response> UpdateFavoritesAsync(UpdateFavoritesRequest request) =>
        await ApiClient.SendAsync(
            new HttpRequest(
                Url: $"{Configuration[Key]}/api/v1/Favorites/update-favorites",
                Method: HttpMethod.Put, Data: request),
            new HttpOptions() { ValidateToken = true, WithBearer = true });
}