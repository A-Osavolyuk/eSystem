namespace eShop.Product.Api.Entities;

public class CategoryEntity : Entity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}