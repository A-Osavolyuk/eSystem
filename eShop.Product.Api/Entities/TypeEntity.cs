namespace eShop.Product.Api.Entities;

public class TypeEntity : IEntity
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset? CreateDate { get; set; }
    public DateTimeOffset? UpdateDate { get; set; }

    public CategoryEntity? Category { get; set; }
}