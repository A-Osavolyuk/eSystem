using eSystem.Core.Server.Data.Entities;

namespace eSecurity.Idp.Data.Entities;

public sealed class PushedAuthorizationRequestScopeEntity : Entity
{
    public Guid Id { get; set; }

    public required string Scope { get; set; }
    
    public Guid RequestId { get; set; }
    public PushedAuthorizationRequestEntity Request { get; set; } = null!;
}