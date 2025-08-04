namespace eShop.Auth.Api.Entities;

public class OAuthProviderEntity : Entity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}