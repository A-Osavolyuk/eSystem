using eSystem.Core.Data.Entities;

namespace eCinema.Server.Data.Entities;

public sealed class SessionClaimEntity : Entity
{
    public required Guid Id { get; set; }
    public required Guid SessionId { get; set; }

    public required string Type { get; set; }
    public required string Value { get; set; }

    public SessionEntity Session { get; set; } = null!;
}