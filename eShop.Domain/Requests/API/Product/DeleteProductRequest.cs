namespace eShop.Domain.Requests.API.Product;

public record DeleteProductRequest()
{
    public Guid ProductId { get; set; }
}