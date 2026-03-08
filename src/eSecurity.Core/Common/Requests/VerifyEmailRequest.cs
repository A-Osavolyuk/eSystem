namespace eSecurity.Core.Common.Requests;

public sealed class VerifyEmailRequest
{
    [JsonPropertyName("email")]
    public required string Email { get; set; }
}