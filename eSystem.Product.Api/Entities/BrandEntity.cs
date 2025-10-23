namespace eSystem.Product.Api.Entities;

public class BrandEntity : Entity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public string? WebsiteUrl { get; set; } = string.Empty;
    public string? LogoUrl { get; set; } = string.Empty;
    public string? Country { get; set; } = string.Empty;
}