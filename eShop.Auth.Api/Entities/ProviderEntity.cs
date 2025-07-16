namespace eShop.Auth.Api.Entities;

public class ProviderEntity : Entity
{
    public Guid Id { get; init; }
    public string Name { get; set; } = string.Empty;
}
