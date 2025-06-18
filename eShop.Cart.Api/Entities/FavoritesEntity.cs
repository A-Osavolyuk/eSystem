using eShop.Domain.Abstraction.Data;
using eShop.Domain.Types;

namespace eShop.Cart.Api.Entities;

public class FavoritesEntity : IEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; init; }
    [BsonRepresentation(BsonType.String)] public Guid UserId { get; init; }
    public int ItemsCount { get; init; }
    public List<FavoritesItem> Items { get; init; } = [];
    public DateTimeOffset? CreateDate { get; set; }
    public DateTimeOffset? UpdateDate { get; set; }
}