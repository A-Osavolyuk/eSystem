namespace eShop.Domain.Requests.Api.Brand;

public record DeleteBrandRequest
{
    public Guid Id { get; set; }
}