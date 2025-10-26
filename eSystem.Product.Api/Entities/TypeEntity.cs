using eSystem.Core.Data.Entities;

namespace eSystem.Product.Api.Entities;

public class TypeEntity : Entity
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public CategoryEntity? Category { get; set; }
}