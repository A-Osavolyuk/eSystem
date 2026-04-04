namespace eSecurity.Core.Common.DTOs;

public class QrCode
{
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
    
    [JsonPropertyName("secret")]
    public string Secret { get; set; } = string.Empty;
}