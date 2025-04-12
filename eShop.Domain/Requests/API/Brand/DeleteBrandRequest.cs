namespace eShop.Domain.Requests.API.Brand;

public record DeleteBrandRequest
{
    public Guid Id { get; set; }
}