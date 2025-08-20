using System.Text.Json.Serialization;

namespace eShop.Auth.Api.Types;

public class ClientData
{
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AuthType Type { get; set; }
    
    [JsonPropertyName("challenge")]
    public string Challenge { get; set; } = string.Empty;
    
    [JsonPropertyName("origin")]
    public string Origin { get; set; } = string.Empty;
    
    [JsonPropertyName("crossOrigin")]
    public bool CrossOrigin { get; set; }
}