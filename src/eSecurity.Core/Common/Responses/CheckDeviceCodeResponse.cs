namespace eSecurity.Core.Common.Responses;

public sealed class CheckDeviceCodeResponse
{
    [JsonPropertyName("exists")]
    public bool Exists { get; set; }
    
    [JsonPropertyName("is_denied")]
    public bool IsDenied { get; set; }
    
    [JsonPropertyName("is_activated")]
    public bool IsActivated { get; set; }
    
    [JsonPropertyName("is_consumed")]
    public bool IsConsumed { get; set; }
    
    [JsonPropertyName("is_expired")]
    public bool IsExpired { get; set; }
}