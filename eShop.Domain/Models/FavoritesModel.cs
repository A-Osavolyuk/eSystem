namespace eShop.Domain.Models;

public class FavoritesModel : IIdentifiable<Guid>
{
    public Guid Id { get; init; }
    public int ItemsCount { get; set; }
    public List<FavoritesItem> Items { get; set; } = new List<FavoritesItem>();

    public void Count()
    {
        ItemsCount = Items.Count();
    }
}