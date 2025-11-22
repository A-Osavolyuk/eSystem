using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public class OpaqueTokenEntity : Entity
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public Guid UserId { get; set; }

    public required string Token { get; set; }
    public bool Revoked { get; set; }
    public DateTimeOffset? RevokedDate { get; set; }
    public DateTimeOffset ExpiredDate { get; set; }
    
    public ClientEntity Client { get; set; } = null!;
    public UserEntity User { get; set; } = null!;
    public ICollection<OpaqueTokenScopeEntity> Scopes { get; set; } = null!;
}