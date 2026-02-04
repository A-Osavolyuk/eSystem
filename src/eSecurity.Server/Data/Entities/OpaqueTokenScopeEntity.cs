using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public class OpaqueTokenScopeEntity : Entity
{
    public Guid Id { get; set; }

    public Guid ClientScopeId { get; set; }
    public ClientAllowedScopeEntity ClientScope { get; set; } = null!;

    public Guid TokenId { get; set; }
    public OpaqueTokenEntity Token { get; set; } = null!;
}