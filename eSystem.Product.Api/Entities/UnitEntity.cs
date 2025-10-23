namespace eSystem.Product.Api.Entities;

public class UnitEntity : Entity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}