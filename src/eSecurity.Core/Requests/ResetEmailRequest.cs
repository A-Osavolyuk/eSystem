using System.Text.Json.Serialization;

namespace eSecurity.Core.Requests;

public sealed class ResetEmailRequest
{
    [JsonPropertyName("verification_id")]
    public required Guid VerificationId { get; set; }
    
    [JsonPropertyName("email")]
    public required string Email { get; set; }
}