using eShop.Domain.Abstraction.Data;

namespace eShop.Domain.Requests.API.Cart;

public record UpdateFavoritesRequest
{
    public Guid Id { get; init; }
    public int ItemsCount { get; set; }
    public List<FavoritesItem> Items { get; set; } = [];
}