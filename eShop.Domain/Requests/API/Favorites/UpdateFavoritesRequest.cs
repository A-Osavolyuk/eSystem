namespace eShop.Domain.Requests.Api.Favorites;

public record UpdateFavoritesRequest : IIdentifiable<Guid>
{
    public Guid Id { get; init; }
    public int ItemsCount { get; set; }
    public List<FavoritesItem> Items { get; set; } = new();
}