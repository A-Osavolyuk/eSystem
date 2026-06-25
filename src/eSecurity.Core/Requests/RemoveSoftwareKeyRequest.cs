using System.Text.Json.Serialization;

namespace eSecurity.Core.Requests;

public sealed class RemoveSoftwareKeyRequest
{
    [JsonPropertyName("verification_id")]
    public required Guid VerificationId { get; set; }
    
    [JsonPropertyName("passkey_id")]
    public Guid PasskeyId { get; set; }
}