using eShop.Domain.Abstraction.Data;
using eShop.Domain.Types;

namespace eShop.Cart.Api.Entities;

public class CartEntity : IEntity<Guid>
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; init; }

    [BsonRepresentation(BsonType.String)] public Guid UserId { get; init; }
    public int ItemsCount { get; init; }
    public List<CartItem> Items { get; init; } = [];
    public DateTimeOffset? CreateDate { get; set; }
    public DateTimeOffset? UpdateDate { get; set; }
}