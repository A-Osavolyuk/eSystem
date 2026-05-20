using System.Text.Json.Serialization;

namespace eSecurity.Core.Responses;

public sealed class VerificationResponse
{
    [JsonPropertyName("verification_id")]
    public Guid VerificationId { get; set; }
}