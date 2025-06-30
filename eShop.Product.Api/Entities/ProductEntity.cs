using eShop.Domain.Abstraction.Data;

namespace eShop.Product.Api.Entities;

public class ProductEntity : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public long Article { get; set; }
    public decimal Price { get; set; }
    public int QuantityInStock { get; set; }
    public Guid TypeId { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public PricePerUnitType PricePerUnitType { get; set; }

    public DateTimeOffset? CreateDate { get; set; }
    public DateTimeOffset? UpdateDate { get; set; }

    public ProductTypeEntity? Type { get; set; } 
}