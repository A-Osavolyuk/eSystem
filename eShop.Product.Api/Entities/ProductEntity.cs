using eShop.Domain.Abstraction.Data;

namespace eShop.Product.Api.Entities;

public class ProductEntity : IEntity<Guid>
{
    public ProductEntity() => Article = GenerateArticle();
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public ProductTypes ProductType { get; set; } = ProductTypes.None;
    public string Article { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public Currency Currency { get; set; }
    public List<string> Images { get; set; } = [];
    public Guid BrandId { get; set; }
    public Guid SellerId { get; set; }
    public BrandEntity Brand { get; set; } = new();
    public SellerEntity Seller { get; set; } = new();
    public DateTime CreateDate { get; set; }
    public DateTime UpdateDate { get; set; }
    private static string GenerateArticle() => new Random().NextInt64(100_000_000, 999_999_999_999).ToString();
}