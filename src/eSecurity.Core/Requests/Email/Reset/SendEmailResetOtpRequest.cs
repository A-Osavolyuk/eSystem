using System.Text.Json.Serialization;

namespace eSecurity.Core.Requests.Email.Reset;

public sealed class SendEmailResetOtpRequest
{
    [JsonPropertyName("current_email")]
    public required string CurrentEmail { get; set; }
    
    [JsonPropertyName("new_email")]
    public required string NewEmail { get; set; }
}