using System.Text.Json.Serialization;
using eSystem.Core.Data.Entities;

namespace eSecurity.Client.BFF.Data.Entities;

public sealed class SessionClaimEntity : Entity
{
    public Guid Id { get; set; }
    
    public required string Name { get; set; }
    public required string Value { get; set; }

    public Guid SessionId { get; set; }
    public SessionEntity Session { get; set; } = null!;
}