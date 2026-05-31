using System.Text.Json.Serialization;

namespace eSecurity.Core.Requests.Email;

public sealed class ConfirmEmailVerificationRequest
{
    [JsonPropertyName("email")] 
    public string? Email { get; set; }
    
    [JsonPropertyName("code")] 
    public string? Code { get; set; }
}