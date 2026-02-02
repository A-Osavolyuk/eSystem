namespace eSecurity.Core.Common.DTOs;

public class UserDeviceDto
{
    public Guid Id { get; set; }
    
    public bool IsBlocked { get; set; }
    
    public string? UserAgent { get; set; } =  string.Empty;
    public string? IpAddress { get; set; } = string.Empty;
    public string? Browser { get; set; }
    public string? Device { get; set; }
    public string? Os { get; set; }
    public string? Location { get; set; }

    public DateTimeOffset FirstSeen { get; set; }
    public DateTimeOffset? LastSeen { get; set; }
    public DateTimeOffset? BlockedDate { get; set; }
}