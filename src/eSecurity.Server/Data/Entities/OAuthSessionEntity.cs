using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public sealed class OAuthSessionEntity : Entity
{
    public Guid Id { get; set; }
    
    public required string Provider { get; set; }
    public required string[] Amr { get; set; }
    public required DateTimeOffset ExpiredAt { get; set; }
    public bool IsValid => ExpiredAt > DateTimeOffset.UtcNow;
    
    public bool? RequireTwoFactor { get; set; }
    public OAuthFlow? Flow { get; set; }
    
    public Guid? UserId { get; set; }
    public UserEntity? User { get; set; }
}