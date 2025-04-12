using eShop.Domain.Common.Api;
using eShop.Domain.Requests.Api.Favorites;

namespace eShop.Infrastructure.Services;

public class FavoritesService(
    IHttpClientService clientService,
    IConfiguration configuration) : IFavoritesService
{
    private readonly IHttpClientService clientService = clientService;
    private readonly IConfiguration configuration = configuration;

    public async ValueTask<Response> GetFavoritesAsync(Guid userId) => await clientService.SendAsync(
        new Request(Url: $"{configuration["Configuration:Services:Proxy:Gateway:Uri"]}/api/v1/Favorites/get-favorites/{userId}", Methods: HttpMethods.Get));

    public async ValueTask<Response> UpdateFavoritesAsync(UpdateFavoritesRequest request) => await clientService.SendAsync(
        new Request(Url: $"{configuration["Configuration:Services:Proxy:Gateway:Uri"]}/api/v1/Favorites/update-favorites", Methods: HttpMethods.Put, Data: request));
}