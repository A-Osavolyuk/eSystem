namespace eShop.Domain.DTOs;

public class LockoutStateDto
{
    public LockoutReason Reason { get; set; }
    public string? Description { get; set; }
    public bool Enabled { get; set; }
    public bool Permanent { get; set; }
    public bool IsActive => Enabled && EndDate > DateTimeOffset.UtcNow || Permanent;
    
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
}