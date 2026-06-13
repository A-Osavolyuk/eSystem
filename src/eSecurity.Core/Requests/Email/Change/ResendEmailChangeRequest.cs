using System.Text.Json.Serialization;

namespace eSecurity.Core.Requests.Email.Change;

public sealed class ResendEmailChangeRequest
{
    [JsonPropertyName("new_email")]
    public required string NewEmail { get; set; }
}