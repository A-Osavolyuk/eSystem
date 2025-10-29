using eSystem.Core.Data.Entities;

namespace eSystem.Auth.Api.Data.Entities;

public class UserDeviceEntity : Entity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public bool IsTrusted { get; set; }
    public bool IsBlocked { get; set; }
    
    public required string UserAgent { get; set; }
    public required string IpAddress { get; set; }
    public required string Browser { get; set; }
    public required string Device { get; set; }
    public required string OS { get; set; }
    public string? Location { get; set; }

    public DateTimeOffset FirstSeen { get; set; }
    public DateTimeOffset? LastSeen { get; set; }
    public DateTimeOffset? BlockedDate { get; set; }

    public UserEntity User { get; set; } = null!;
    public PasskeyEntity? Passkey { get; set; }
    public ICollection<SessionEntity> Sessions { get; set; } = null!;
}