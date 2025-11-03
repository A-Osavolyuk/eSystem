using eSystem.Core.Data.Entities;

namespace eSecurity.Data.Entities;

public class GrantedScopeEntity : Entity
{
    public Guid ConsentId { get; set; }
    public Guid ScopeId { get; set; }

    public ConsentEntity Consent { get; set; } = null!;
    public ScopeEntity Scope { get; set; } = null!;
}