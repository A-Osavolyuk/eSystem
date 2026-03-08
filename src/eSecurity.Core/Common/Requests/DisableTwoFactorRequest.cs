namespace eSecurity.Core.Common.Requests;

public sealed class DisableTwoFactorRequest
{
    [JsonPropertyName("verification_id")]
    public required Guid VerificationId { get; set; }
}