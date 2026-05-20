using System.Text.Json.Serialization;

namespace eSecurity.Core.Requests;

public sealed class DisableTwoFactorRequest
{
    [JsonPropertyName("verification_id")]
    public required Guid VerificationId { get; set; }
}