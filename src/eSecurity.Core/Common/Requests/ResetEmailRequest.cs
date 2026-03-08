namespace eSecurity.Core.Common.Requests;

public sealed class ResetEmailRequest
{
    [JsonPropertyName("email")]
    public required string Email { get; set; }
}