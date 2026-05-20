using System.Text.Json.Serialization;

namespace eSecurity.Core.Requests;

public sealed class EnableTwoFactorRequest
{
    [JsonPropertyName("verification_id")]
    public required Guid VerificationId { get; set; }
}