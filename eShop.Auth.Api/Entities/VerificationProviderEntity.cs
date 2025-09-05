namespace eShop.Auth.Api.Entities;

public class VerificationProviderEntity : Entity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}