using System.Text.Json.Serialization;
using eShop.Domain.Common.Security.Constants;

namespace eShop.Domain.Common.Security.Credentials;

public class PublicKeyCredentialRequestOptions
{
    [JsonPropertyName("challenge")]
    public required string Challenge { get; set; }
    
    [JsonPropertyName("timeout")]
    public required int Timeout { get; set; }
    
    [JsonPropertyName("rpId")]
    public required string Domain { get; set; }

    [JsonPropertyName("userVerification")]
    public required string UserVerification { get; set; } = UserVerifications.Preferred;
    
    [JsonPropertyName("allowCredentials")]
    public required List<PublicKeyCredentialDescriptor> AllowCredentials { get; set; }
}

public class PublicKeyCredentialDescriptor
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    
    [JsonPropertyName("type")]
    public required string Type { get; set; }

    [JsonPropertyName("transports")]
    public List<string>? Transports { get; set; }
}