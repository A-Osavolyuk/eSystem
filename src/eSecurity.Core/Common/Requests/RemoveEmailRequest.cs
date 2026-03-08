namespace eSecurity.Core.Common.Requests;

public sealed class RemoveEmailRequest
{
    [JsonPropertyName("email")]
    public required string Email { get; set; }
}