namespace eShop.Product.Api.Entities;

public class ProductEntity : Entity
{
    public Guid Id { get; set; }
    public Guid TypeId { get; set; }
    public Guid UnitId { get; set; }
    public Guid PriceTypeId { get; set; }
    public Guid CurrencyId { get; set; }
    public Guid BrandId { get; set; }
    public Guid SupplierId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public long Article { get; set; }
    public decimal Price { get; set; }
    public int QuantityInStock { get; set; }
    public TypeEntity? Type { get; set; }
    public UnitEntity? Unit { get; set; }
    public PriceTypeEntity? PriceType { get; set; }
    public CurrencyEntity? Currency { get; set; }
    public BrandEntity? Brand { get; set; }
    public SupplierEntity? Supplier { get; set; }
}