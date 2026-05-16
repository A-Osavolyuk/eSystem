using eSystem.Core.Data.Entities;

namespace eSecurity.Client.BFF.Data.Entities;

public sealed class SessionEntity : Entity
{
    public Guid Id { get; set; }
    
    public required string Key { get; set; }
    public required string UserId { get; set; }
    public required string Sid { get; set; }

    public required SessionPropertiesEntity Properties { get; set; }
    public required ICollection<SessionClaimEntity> Claim { get; set; }
    public required ICollection<SessionTokenEntity> Tokens { get; set; }
}