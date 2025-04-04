namespace eShop.Domain.Requests.Api.Brand;

public record UpdateBrandRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}