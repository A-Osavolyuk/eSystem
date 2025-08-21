using System.Text.Json.Serialization;
using eShop.Domain.Constants;

namespace eShop.Domain.Types;

public class CredentialRequestOptions
{
    [JsonPropertyName("challenge")]
    public required string Challenge { get; set; }
    
    [JsonPropertyName("timeout")]
    public required int Timeout { get; set; }
    
    [JsonPropertyName("rpId")]
    public required string Domain { get; set; }

    [JsonPropertyName("userVerification")]
    public required string UserVerification { get; set; } = UserVerifications.Preferred;
}