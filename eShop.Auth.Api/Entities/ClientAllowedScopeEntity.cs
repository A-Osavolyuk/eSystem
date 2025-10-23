namespace eShop.Auth.Api.Entities;

public class ClientAllowedScopeEntity : Entity
{
    public Guid ClientId { get; set; }
    public Guid ScopeId { get; set; }

    public ClientEntity Client { get; set; } = null!;
    public ScopeEntity Scope { get; set; } = null!;
}