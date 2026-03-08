namespace eSecurity.Core.Common.Requests;

public sealed class ReconfigureAuthenticatorRequest
{
    [JsonPropertyName("verification_id")]
    public required Guid VerificationId { get; set; }
    
    [JsonPropertyName("secret")]
    public required string Secret { get; set; }
}