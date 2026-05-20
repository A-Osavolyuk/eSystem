using eSystem.Core.Server.Data.Entities;

namespace eSecurity.Idp.Data.Entities;

public class GrantedScopeEntity : Entity
{
    public required Guid Id { get; set; }
    
    public required Guid ConsentId { get; set; }
    public ConsentEntity Consent { get; set; } = null!;
    
    public Guid ClientScopeId { get; set; }
    public ClientAllowedScopeEntity ClientScope { get; set; } = null!;

}