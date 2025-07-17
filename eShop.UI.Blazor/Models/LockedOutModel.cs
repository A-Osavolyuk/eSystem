using eShop.Domain.Enums;

namespace eShop.BlazorWebUI.Models;

public class LockoutModel
{
    public string? Reason { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public bool Enabled { get; set; }
    public bool Permanent { get; set; }
    public bool IsActive => Enabled && EndDate > DateTimeOffset.UtcNow || Permanent;
    
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
}