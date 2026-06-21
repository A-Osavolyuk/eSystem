using System.Text.Json.Serialization;

namespace eSecurity.Core.Requests.Verification;

public sealed class VerifyEmailOtpRequest : VerificationRequest
{
    [JsonPropertyName("verification_id")]
    public Guid VerificationId { get; set; }
    
    [JsonPropertyName("code")]
    public required string Code { get; set; }
}