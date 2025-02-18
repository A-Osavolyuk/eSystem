using eShop.Domain.Types;

namespace eShop.Cart.Api.Entities;

public class FavoritesEntity : IIdentifiable<Guid>, IAuditable
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; init; }
    [BsonRepresentation(BsonType.String)] public Guid UserId { get; init; }
    public int ItemsCount { get; init; }
    public List<FavoritesItem> Items { get; init; } = new List<FavoritesItem>();
    public DateTime CreateDate { get; init; }
    public DateTime UpdateDate { get; init; }
}