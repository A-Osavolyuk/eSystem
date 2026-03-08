namespace eSecurity.Core.Common.Requests;

public sealed class VerifyEmailRequest
{
    [JsonPropertyName("verification_id")]
    public required Guid VerificationId { get; set; }
    
    [JsonPropertyName("email")]
    public required string Email { get; set; }
}