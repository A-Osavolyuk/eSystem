namespace eShop.Domain.Requests.API.Cart;

public record UpdateCartRequest
{
    public Guid Id { get; init; }
    public int ItemsCount { get; set; }
    public List<CartItem> Items { get; set; } = [];
}