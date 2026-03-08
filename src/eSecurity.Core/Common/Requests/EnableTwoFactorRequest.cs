namespace eSecurity.Core.Common.Requests;

public sealed class EnableTwoFactorRequest
{
    [JsonPropertyName("verification_id")]
    public required Guid VerificationId { get; set; }
}