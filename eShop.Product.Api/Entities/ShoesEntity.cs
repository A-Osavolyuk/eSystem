namespace eShop.Product.Api.Entities;

public class ShoesEntity : ProductEntity
{
    public ProductColor Color { get; set; } = ProductColor.None;
    public List<Size> Size { get; set; } = new List<Size>();
    public ProductAudience ProductAudience { get; set; } = ProductAudience.None;
}