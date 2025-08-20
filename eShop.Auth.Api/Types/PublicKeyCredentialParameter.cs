using System.Text.Json.Serialization;

namespace eShop.Auth.Api.Types;

public class PublicKeyCredentialParameter
{
    [JsonPropertyName("type")]
    public KeyType Type { get; set; }
    
    [JsonPropertyName("alg")]
    [JsonConverter(typeof(AlgorithmConverter))]
    public Algorithm Algorithm { get; set; }
}