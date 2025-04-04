namespace eShop.Domain.Requests.Api.Product;

public record CreateProductRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public ProductTypes ProductType { get; set; } = ProductTypes.None;
    public string Article { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public ProductCurrency ProductCurrency { get; set; }
    public List<string> Images { get; set; } = new List<string>();
    public BrandDto Brand { get; set; } = new BrandDto();
    public SellerDto Seller { get; set; } = new SellerDto();
    public ProductColor Color { get; set; } = ProductColor.None;
    public IEnumerable<ProductSize> Size { get; set; } = Enumerable.Empty<ProductSize>();
    public ProductAudience ProductAudience { get; set; } = ProductAudience.None;
};