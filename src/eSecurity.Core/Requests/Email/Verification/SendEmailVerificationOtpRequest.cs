using System.Text.Json.Serialization;

namespace eSecurity.Core.Requests.Email.Verification;

public class SendEmailVerificationOtpRequest
{
    [JsonPropertyName("email")] 
    public string? Email { get; set; }
}