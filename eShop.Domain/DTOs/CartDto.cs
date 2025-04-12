using eShop.Domain.Abstraction.Data;

namespace eShop.Domain.DTOs;

public class CartDto : IIdentifiable<Guid>
{
    public Guid Id { get; init; }
    public int Count { get; set; }
    public List<CartItem> Items { get; set; } = new();
}