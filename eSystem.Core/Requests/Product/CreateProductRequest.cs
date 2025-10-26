namespace eSystem.Core.Requests.Product;

public class CreateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public int QuantityInStock { get; set; }
    public decimal Price { get; set; }
    public Guid TypeId { get; set; }
    public Guid UnitId { get; set; }
    public Guid PriceTypeId { get; set; }
    public Guid CurrencyId { get; set; }
    public Guid BrandId { get; set; }
    public Guid SupplierId { get; set; }
    public Dictionary<string, object> Properties { get; set; } = [];
}