using eSecurity.Security.Authentication.Lockout;

namespace eSecurity.Common.DTOs;

public class LockoutStateDto
{
    public Guid Id { get; set; }
    public LockoutType Type { get; set; }
    public string? Description { get; set; }
    public bool Enabled { get; set; }
    public bool Permanent { get; set; }
    public TimeSpan? Duration { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
}