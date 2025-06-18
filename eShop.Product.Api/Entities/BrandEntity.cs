using eShop.Domain.Abstraction.Data;

namespace eShop.Product.Api.Entities;

public class BrandEntity : IEntity
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public DateTimeOffset? CreateDate { get; set; }
    public DateTimeOffset? UpdateDate { get; set; }
}