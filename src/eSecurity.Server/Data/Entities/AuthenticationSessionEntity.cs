using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public sealed class AuthenticationSessionEntity : Entity
{
    public Guid Id { get; set; }

    public string? IdentityProvider { get; set; }
    public OAuthFlow? OAuthFlow { get; set; }
    
    public string[] PassedAuthenticationMethods { get; set; } = [];
    public string[] RequiredAuthenticationMethods { get; set; } = [];
    
    public bool IsRevoked { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    
    public DateTimeOffset ExpiredAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    
    public bool IsActive => !IsRevoked && ExpiredAt > DateTimeOffset.UtcNow;

    public Guid? UserId { get; set; }
    public UserEntity? User { get; set; }
    
    public Guid? SessionId { get; set; }
    public SessionEntity? Session { get; set; }
}