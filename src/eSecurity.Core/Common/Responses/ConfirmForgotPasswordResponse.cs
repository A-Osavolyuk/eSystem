namespace eSecurity.Core.Common.Responses;

public sealed class ConfirmForgotPasswordResponse
{
    [JsonPropertyName("verification_id")]
    public Guid VerificationId { get; set; }
}