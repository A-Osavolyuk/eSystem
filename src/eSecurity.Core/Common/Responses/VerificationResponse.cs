using System.Text.Json.Serialization;

namespace eSecurity.Core.Common.Responses;

public sealed class VerificationResponse
{
    [JsonPropertyName("verification_id")]
    public Guid VerificationId { get; set; }
}