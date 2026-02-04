using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public class ClientAllowedScopeEntity : Entity
{
    public required Guid Id { get; set; }
    public required Guid ClientId { get; set; }
    public ClientEntity Client { get; set; } = null!;
    
    public required Guid ScopeId { get; set; }
    public ScopeEntity Scope { get; set; } = null!;
}