using eShop.Domain.DTOs;
using eShop.Domain.Enums;

namespace eShop.BlazorWebUI.Models;

public class ProductModel
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int QuantityInStock { get; set; }
    public TypeModel Type { get; set; } = new();
    public UnitModel Unit { get; set; } = new();
    public PriceTypeModel PriceType { get; set; } = new();
    public Dictionary<string, object> Properties { get; set; } = [];
    public List<TypeDto> Types { get; set; } = [];
    public List<UnitDto> Units { get; set; } = [];
    public List<PriceTypeDto> PriceTypes { get; set; } = [];
}