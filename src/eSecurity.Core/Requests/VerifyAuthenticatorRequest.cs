using System.Text.Json.Serialization;

namespace eSecurity.Core.Requests;

public sealed class VerifyAuthenticatorRequest
{
    [JsonPropertyName("code")]
    public required string Code { get; set; }
    
    [JsonPropertyName("secret")]
    public required string Secret { get; set; }
}