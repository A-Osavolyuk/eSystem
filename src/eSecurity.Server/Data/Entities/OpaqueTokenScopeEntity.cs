using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public class OpaqueTokenScopeEntity : Entity
{
    public Guid Id { get; set; }
    public Guid TokenId { get; set; }
    public required string Scope { get; set; }

    public OpaqueTokenEntity Token { get; set; } = null!;
}