namespace eShop.Product.Api.Entities;

public class BrandEntity : IIdentifiable<Guid>, IAuditable
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public DateTime CreateDate { get; init; }
    public DateTime UpdateDate { get; init; }
}