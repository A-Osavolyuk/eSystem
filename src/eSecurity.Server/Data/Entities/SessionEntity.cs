using eSystem.Core.Server.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public sealed class SessionEntity : Entity
{
    public Guid Id { get; set; }
    
    public required string Key { get; set; }
    public required string UserId { get; set; }
    public required string Sid { get; set; }

    public SessionPropertiesEntity Properties { get; set; } = null!;
    public ICollection<SessionClaimEntity> Claims { get; set; } = [];
    public ICollection<SessionTokenEntity> Tokens { get; set; } = [];
}