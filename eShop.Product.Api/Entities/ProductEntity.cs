using eShop.Domain.Abstraction.Data;
using eShop.Domain.Enums;

namespace eShop.Product.Api.Entities;

public class ProductEntity : IEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public long Article { get; set; }
    
    public ProductType ProductType { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public PricePerUnitType PricePerUnitType { get; set; }
    
    public decimal Price { get; set; }
    public int QuantityInStock { get; set; }

    public DateTimeOffset? CreateDate { get; set; }
    public DateTimeOffset? UpdateDate { get; set; }
}