namespace eShop.Blazor.Server.Domain.Models;

public class CurrencyModel
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Sign { get; set; } = string.Empty;
}