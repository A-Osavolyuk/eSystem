using System.Text.Json.Serialization;
using eSecurity.Core.Security.Authorization.Verification;

namespace eSecurity.Core.Requests.Verification;

public sealed class SendEmailOtpRequest
{
    [JsonPropertyName("email")]
    public required string Email { get; set; }
    
    [JsonPropertyName("operation_type")]
    public required OperationType OperationType { get; set; }
}