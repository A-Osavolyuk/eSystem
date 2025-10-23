namespace eSystem.Product.Api.Entities;

public class CurrencyEntity : Entity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Sign { get; set; } = string.Empty;
}