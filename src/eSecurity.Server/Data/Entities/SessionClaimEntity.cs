using eSystem.Core.Server.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public sealed class SessionClaimEntity : Entity
{
    public Guid Id { get; set; }
    
    public required string Type { get; set; }
    public required string Value { get; set; }

    public Guid SessionId { get; set; }
    public SessionEntity Session { get; set; } = null!;
}