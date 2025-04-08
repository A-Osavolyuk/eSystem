namespace eShop.Domain.DTOs;

public record ProductDto : IIdentifiable<Guid>
{
    public Guid Id { get; init; }
    public ProductTypes Type { get; set; } = ProductTypes.None;
    public string Article { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public Currency Currency { get; set; }
    public List<string> Images { get; set; } = new();
    public BrandDto Brand { get; set; } = new();
    public SellerDto Seller { get; set; } = new();
}