using System.Text.Json.Serialization;

namespace eSecurity.Core.Requests.Email.Change;

public sealed class SendEmailChangeOtpRequest
{
    [JsonPropertyName("new_email")]
    public required string NewEmail { get; set; }
}