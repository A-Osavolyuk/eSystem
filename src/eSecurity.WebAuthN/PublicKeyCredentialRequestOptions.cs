using System.Text.Json.Serialization;
using eSecurity.WebAuthN.Constants;

namespace eSecurity.WebAuthN;

public class PublicKeyCredentialRequestOptions
{
    [JsonPropertyName("challenge")]
    public required string Challenge { get; set; }
    
    [JsonPropertyName("timeout")]
    public required int Timeout { get; set; }
    
    [JsonPropertyName("rpId")]
    public required string Domain { get; set; }

    [JsonPropertyName("userVerification")]
    public required UserVerification UserVerification { get; set; } = UserVerification.Preferred;

    [JsonPropertyName("allowCredentials")]
    public List<PublicKeyCredentialDescriptor> AllowCredentials { get; set; } = [];
}

public class PublicKeyCredentialDescriptor
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    
    [JsonPropertyName("type")]
    public KeyType Type { get; set; }

    [JsonPropertyName("transports")]
    public List<CredentialTransport>? Transports { get; set; }
}