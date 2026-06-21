using System.Text.Json.Serialization;

namespace eSecurity.Core.Requests.Email.Verification;

public sealed class VerifyEmailRequest
{
    [JsonPropertyName("verification_id")]
    public required Guid VerificationId { get; set; }
    
    [JsonPropertyName("email")] 
    public required string Email { get; set; }
    
    [JsonPropertyName("code")] 
    public required string Code { get; set; }
}