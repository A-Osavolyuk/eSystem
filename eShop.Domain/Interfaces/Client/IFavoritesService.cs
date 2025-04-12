using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Favorites;

namespace eShop.Domain.Interfaces.Client;

public interface IFavoritesService
{
    public ValueTask<Response> GetFavoritesAsync(Guid userId);
    public ValueTask<Response> UpdateFavoritesAsync(UpdateFavoritesRequest request);
}