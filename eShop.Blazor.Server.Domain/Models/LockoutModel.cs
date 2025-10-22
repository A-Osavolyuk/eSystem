using eShop.Domain.Security.Lockout;

namespace eShop.Blazor.Server.Domain.Models;

public class LockoutModel
{
    public Guid Id { get; set; }
    public string? Description { get; set; }
    public bool Enabled { get; set; }
    public bool Permanent { get; set; }
    public LockoutType? Type { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
}