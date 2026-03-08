namespace eSecurity.Core.Common.Requests;

public sealed class CheckEmailRequest
{
    [JsonPropertyName("email")]
    public required string Email { get; set; }
}