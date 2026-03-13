using eSecurity.Core.Security.Authentication.Lockout;
using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public class UserLockoutStateEntity : Entity
{
    public Guid Id { get; init; }

    public bool Enabled => EndedAt > DateTimeOffset.UtcNow || Permanent;
    public LockoutType Type { get; set; }
    public string? Description { get; set; }
    public bool Permanent { get; set; }
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? EndedAt { get; set; }
    
    public Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;
}