namespace eShop.Domain.Requests.API.Product;

public class CreateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public int QuantityInStock { get; set; }
    public decimal Price { get; set; }
    public Guid TypeId { get; set; }
    public Guid UnitId { get; set; }
    public PricePerUnitType PricePerUnitType { get; set; }
    public Dictionary<string, object> Properties { get; set; } = [];
}