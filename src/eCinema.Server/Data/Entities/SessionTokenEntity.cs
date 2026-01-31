using eSystem.Core.Data.Entities;

namespace eCinema.Server.Data.Entities;

public sealed class SessionTokenEntity : Entity
{
    public required Guid Id { get; set; }
    public required Guid SessionId { get; set; }

    public required string TokenType { get; set; }
    public required string EncryptedValue  { get; set; }
    public DateTimeOffset? ExpiresUtc { get; set; }

    public SessionEntity Session { get; set; } = null!;
}