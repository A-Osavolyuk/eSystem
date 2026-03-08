namespace eSecurity.Core.Common.Requests;

public sealed class ForgotPasswordRequest
{
    [JsonPropertyName("email")]
    public required string Email { get; set; }
}