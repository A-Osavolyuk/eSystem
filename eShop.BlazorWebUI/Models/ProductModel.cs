using eShop.Domain.DTOs;
using eShop.Domain.Enums;

namespace eShop.BlazorWebUI.Models;

public class ProductModel
{
    public string Name { get; set; } = string.Empty;
    public TypeModel Type { get; set; } = new();
    public decimal Price { get; set; }
    public int QuantityInStock { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public PricePerUnitType PricePerUnitType { get; set; }
    public Dictionary<string, object> Properties { get; set; } = [];
    public List<ProductTypeDto> Types { get; set; } = [];
}