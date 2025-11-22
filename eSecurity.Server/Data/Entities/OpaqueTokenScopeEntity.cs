using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public class OpaqueTokenScopeEntity : Entity
{
    public Guid TokenId { get; set; }
    public Guid ScopeId { get; set; }

    public OpaqueTokenEntity Token { get; set; } = null!;
    public ScopeEntity Scope { get; set; } = null!;
}