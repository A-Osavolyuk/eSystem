using System.Text.Json.Serialization;

namespace eSecurity.Core.Requests.Email.Change;

public sealed class ChangeEmailRequest
{
    [JsonPropertyName("verification_id")]
    public required Guid VerificationId { get; set; }
    
    [JsonPropertyName("code")]
    public required string Code { get; set; }

    [JsonPropertyName("current_email")]
    public required string CurrentEmail { get; set; }
    
    [JsonPropertyName("new_email")]
    public required string NewEmail { get; set; }
}