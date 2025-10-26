using eAccount.Domain.Models.Products;

namespace eAccount.Domain.Models;

public class ProductModel
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public int? QuantityInStock { get; set; }
    public TypeModel Type { get; set; } = new();
    public UnitModel Unit { get; set; } = new();
    public PriceTypeModel PriceType { get; set; } = new();
    public CategoryModel Category { get; set; } = new();
    public CurrencyModel Currency { get; set; } = new();
    public ProductPropertiesModel? Properties { get; set; }
    public List<ImageModel> Images { get; set; } = [];
}