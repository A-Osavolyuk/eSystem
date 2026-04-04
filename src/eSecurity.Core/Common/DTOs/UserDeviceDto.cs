namespace eSecurity.Core.Common.DTOs;

public class UserDeviceDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    
    [JsonPropertyName("is_blocked")]
    public bool IsBlocked { get; set; }
    
    [JsonPropertyName("useragent")]
    public string? UserAgent { get; set; } =  string.Empty;
    
    [JsonPropertyName("id_address")]
    public string? IpAddress { get; set; } = string.Empty;
    
    [JsonPropertyName("browser")]
    public string? Browser { get; set; }
    
    [JsonPropertyName("device")]
    public string? Device { get; set; }
    
    [JsonPropertyName("os")]
    public string? Os { get; set; }
    
    [JsonPropertyName("location")]
    public string? Location { get; set; }

    [JsonPropertyName("first_seen_at")]
    public DateTimeOffset FirstSeenAt { get; set; }
    
    [JsonPropertyName("last_seen_at")]
    public DateTimeOffset? LastSeenAt { get; set; }
    
    [JsonPropertyName("blocked_at")]
    public DateTimeOffset? BlockedAt { get; set; }
}