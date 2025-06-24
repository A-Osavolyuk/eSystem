namespace eShop.Domain.DTOs;

public class BrandDto
{
    public Guid Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}