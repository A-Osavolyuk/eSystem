namespace eShop.Product.Api.Entities;

public class PriceTypeEntity : Entity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}