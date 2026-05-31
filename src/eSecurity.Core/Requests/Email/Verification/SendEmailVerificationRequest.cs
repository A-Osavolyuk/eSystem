using System.Text.Json.Serialization;

namespace eSecurity.Core.Requests.Email.Verification;

public class SendEmailVerificationRequest
{
    [JsonPropertyName("email")] 
    public string? Email { get; set; }
}