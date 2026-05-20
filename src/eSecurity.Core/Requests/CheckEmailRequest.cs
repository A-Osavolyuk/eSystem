using System.Text.Json.Serialization;

namespace eSecurity.Core.Requests;

public sealed class CheckEmailRequest
{
    [JsonPropertyName("email")]
    public required string Email { get; set; }
}