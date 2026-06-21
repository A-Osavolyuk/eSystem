using System.Text.Json.Serialization;

namespace eSecurity.Core.Requests.Verification;

public sealed class ResendEmailOtpRequest
{
    [JsonPropertyName("verification_id")]
    public Guid VerificationId { get; set; }
}