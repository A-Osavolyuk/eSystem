namespace eShop.Domain.DTOs;

public class LockoutStateDto
{
    public string? Code { get; set; }
    public string? Reason { get; set; }
    public string? Description { get; set; }
    public bool Enabled { get; set; }
    public bool Permanent { get; set; }
    public bool IsActive { get; set; }
    
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
}