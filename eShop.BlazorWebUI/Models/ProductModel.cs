using eShop.Domain.DTOs;
using eShop.Domain.Enums;

namespace eShop.BlazorWebUI.Models;

public class ProductModel
{
    public string Name { get; set; } = string.Empty;
    public TypeModel Type { get; set; } = new();
    public decimal Price { get; set; }
    public int QuantityInStock { get; set; }
    public UnitDto Unit { get; set; } = new();
    public PriceTypeDto PriceType { get; set; } = new();
    public Dictionary<string, object> Properties { get; set; } = [];
    public List<TypeDto> Types { get; set; } = [];
}