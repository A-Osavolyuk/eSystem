using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public class UserDeviceEntity : Entity
{
    public Guid Id { get; set; }
    
    public bool IsBlocked { get; set; }
    
    public required string UserAgent { get; set; }
    public required string IpAddress { get; set; }
    public required string Browser { get; set; }
    public required string Device { get; set; }
    public required string Os { get; set; }
    public string? Location { get; set; }

    public DateTimeOffset FirstSeenAt { get; set; }
    public DateTimeOffset? LastSeenAt { get; set; }
    public DateTimeOffset? BlockedAt { get; set; }

    public Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;
}