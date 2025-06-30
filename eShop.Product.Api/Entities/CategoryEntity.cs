namespace eShop.Product.Api.Entities;

public class CategoryEntity : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    public DateTimeOffset? CreateDate { get; set; }
    public DateTimeOffset? UpdateDate { get; set; }
}