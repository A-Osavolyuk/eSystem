using eShop.Domain.DTOs;
using eShop.Domain.Enums;
using Microsoft.AspNetCore.Components.Forms;

namespace eShop.BlazorWebUI.Models;

public class ProductModel
{
    public string Name { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public int? QuantityInStock { get; set; }
    public TypeModel Type { get; set; } = new();
    public UnitModel Unit { get; set; } = new();
    public PriceTypeModel PriceType { get; set; } = new();
    public CategoryModel Category { get; set; } = new();
    public Dictionary<string, object> Properties { get; set; } = [];
    public List<IBrowserFile> Files { get; set; } = [];
    public List<string> Images { get; set; } = [];
}