using eSystem.Core.Server.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public sealed class SessionTokenEntity : Entity
{
    public Guid Id { get; set; }
    
    public required string TokenType { get; set; }
    public required string EncryptedValue { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }

    public Guid SessionId { get; set; }
    public SessionEntity Session { get; set; } = null!;
}