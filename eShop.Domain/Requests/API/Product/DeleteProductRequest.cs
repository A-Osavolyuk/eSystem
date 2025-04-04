namespace eShop.Domain.Requests.Api.Product;

public record DeleteProductRequest()
{
    public Guid ProductId { get; set; }
}