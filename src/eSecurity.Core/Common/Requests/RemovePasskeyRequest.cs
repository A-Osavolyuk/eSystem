namespace eSecurity.Core.Common.Requests;

public sealed class RemovePasskeyRequest
{
    [JsonPropertyName("verification_id")]
    public required Guid VerificationId { get; set; }
    
    [JsonPropertyName("passkey_id")]
    public Guid PasskeyId { get; set; }
}