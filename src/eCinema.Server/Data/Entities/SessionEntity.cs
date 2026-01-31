using eSystem.Core.Data.Entities;

namespace eCinema.Server.Data.Entities;

public sealed class SessionEntity : Entity
{
    public required Guid Id { get; set; }
    
    public required string SessionKey { get; set; }
    public required string Sid { get; set; }
    public required string UserId { get; set; }

    public SessionPropertiesEntity Properties { get; set; } = null!;
    public ICollection<SessionClaimEntity> Claims { get; set; } = null!;
    public ICollection<SessionTokenEntity> Tokens { get; set; } = null!;
}