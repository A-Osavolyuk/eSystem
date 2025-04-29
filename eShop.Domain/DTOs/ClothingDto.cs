namespace eShop.Domain.DTOs;

public record ClothingDto() : ProductDto
{
    public ProductColor Color { get; set; } = ProductColor.None;
    public List<Size> Size { get; set; } = [];
    public ProductAudience ProductAudience { get; set; } = ProductAudience.None;
}