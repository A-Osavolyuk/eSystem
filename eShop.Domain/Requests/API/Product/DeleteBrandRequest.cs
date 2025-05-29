namespace eShop.Domain.Requests.API.Product;

public record DeleteBrandRequest
{
    public Guid Id { get; set; }
}