using System.Text.Json.Serialization;
using eSystem.Core.Data.Entities;

namespace eSecurity.Client.BFF.Data.Entities;

public sealed class SessionTokenEntity : Entity
{
    public Guid Id { get; set; }
    
    public required string TokenType { get; set; }
    public required string EncryptedValue { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }

    public Guid SessionId { get; set; }
    public SessionEntity Session { get; set; } = null!;
}