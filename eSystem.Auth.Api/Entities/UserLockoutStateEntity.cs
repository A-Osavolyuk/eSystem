using eSystem.Core.Data.Entities;
using eSystem.Core.Security.Authentication.Lockout;

namespace eSystem.Auth.Api.Entities;

public class UserLockoutStateEntity : Entity
{
    public Guid Id { get; init; }
    public Guid UserId { get; set; }

    public bool Enabled => EndDate > DateTimeOffset.UtcNow || Permanent;
    public LockoutType Type { get; set; }
    public string? Description { get; set; }
    public bool Permanent { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    
    public UserEntity User { get; set; } = null!;
}