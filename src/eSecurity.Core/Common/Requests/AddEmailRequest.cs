namespace eSecurity.Core.Common.Requests;

public sealed class AddEmailRequest
{
    [JsonPropertyName("email")]
    public required string Email { get; set; }
}