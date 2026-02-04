using eSecurity.Server.Security.Authorization.Token.DeviceCode;
using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public sealed class DeviceCodeEntity : Entity
{
    public Guid Id { get; set; }

    public required string DeviceCodeHash { get; set; }
    public required string UserCode { get; set; }
    public int Interval { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public DeviceCodeState State { get; set; }
    
    public required string Scope { get; set; }
    public string[]? AcrValues { get; set; }
    
    public Guid ClientId { get; set; }
    public ClientEntity Client { get; set; } = null!;

    public Guid? UserId { get; set; }
    public UserEntity? User { get; set; }
}