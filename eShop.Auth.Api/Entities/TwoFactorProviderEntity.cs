namespace eShop.Auth.Api.Entities;

public class TwoFactorProviderEntity : Entity
{
    public Guid Id { get; init; }
    public string Name { get; set; } = string.Empty;
}
