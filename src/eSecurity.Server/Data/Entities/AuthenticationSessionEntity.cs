using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public sealed class AuthenticationSessionEntity : Entity
{
    public Guid Id { get; set; }

    public string? IdentityProvider { get; set; }
    public List<string> PassedAuthenticationMethods { get; set; } = [];
    public List<string> RequiredAuthenticationMethods { get; set; } = [];
    
    public bool IsRevoked { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    
    public bool IsActive => !IsRevoked && ExpiresAt > DateTimeOffset.UtcNow;

    public Guid? UserId { get; set; }
    public UserEntity? User { get; set; }
}