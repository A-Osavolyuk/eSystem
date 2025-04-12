using eShop.Domain.Abstraction.Data;

namespace eShop.Product.Api.Entities;

public class BrandEntity : IEntity<Guid>
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public DateTime CreateDate { get; set; }
    public DateTime UpdateDate { get; set; }
}