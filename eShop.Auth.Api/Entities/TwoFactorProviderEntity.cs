namespace eShop.Auth.Api.Entities;

public class TwoFactorProviderEntity : Entity
{
    public Guid Id { get; init; }
    public ProviderType Type { get; set; }
}
