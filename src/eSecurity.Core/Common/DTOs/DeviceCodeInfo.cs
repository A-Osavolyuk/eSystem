namespace eSecurity.Core.Common.DTOs;

public sealed class DeviceCodeInfo
{
    [JsonPropertyName("client_name")]
    public required string ClientName { get; set; }
    
    [JsonPropertyName("device_model")]
    public string? DeviceModel { get; set; }
    
    [JsonPropertyName("device_name")]
    public string? DeviceName { get; set; }
    
    [JsonPropertyName("scopes")]
    public required string[] Scopes { get; set; }
}