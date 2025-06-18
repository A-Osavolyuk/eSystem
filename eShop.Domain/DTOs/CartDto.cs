using eShop.Domain.Abstraction.Data;

namespace eShop.Domain.DTOs;

public class CartDto
{
    public Guid Id { get; init; }
    public int Count { get; set; }
    public List<CartItem> Items { get; set; } = [];
}