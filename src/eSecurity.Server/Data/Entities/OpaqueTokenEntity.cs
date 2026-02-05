using eSecurity.Server.Security.Authorization.OAuth.Token;
using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public class OpaqueTokenEntity : Entity
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public Guid? SessionId { get; set; }

    public required OpaqueTokenType TokenType { get; set; }
    public required string TokenHash { get; set; }
    public required string Subject { get; set; }
    public bool Revoked { get; set; }
    public bool IsValid => !Revoked && DateTimeOffset.UtcNow < ExpiredDate;
    
    public DateTimeOffset? RevokedDate { get; set; }
    public DateTimeOffset ExpiredDate { get; set; }
    
    public ClientEntity Client { get; set; } = null!;
    public SessionEntity? Session { get; set; }
    public ICollection<OpaqueTokenScopeEntity> Scopes { get; set; } = null!;
}