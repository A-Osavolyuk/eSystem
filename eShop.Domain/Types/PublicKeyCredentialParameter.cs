using System.Text.Json.Serialization;

namespace eShop.Domain.Types;

public class PublicKeyCredentialParameter
{
    [JsonPropertyName("type")]
    public required string Type { get; set; }
    
    [JsonPropertyName("alg")]
    public int Algorithm { get; set; }
}