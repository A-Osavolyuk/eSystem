namespace eSecurity.Core.Common.Requests;

public sealed class ResetEmailRequest
{
    [JsonPropertyName("verification_id")]
    public required Guid VerificationId { get; set; }
    
    [JsonPropertyName("email")]
    public required string Email { get; set; }
}