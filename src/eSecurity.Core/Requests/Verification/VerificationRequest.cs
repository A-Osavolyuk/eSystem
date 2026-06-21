using System.Text.Json.Serialization;
using eSecurity.Core.Security.Authorization.Verification;

namespace eSecurity.Core.Requests.Verification;

public abstract class VerificationRequest
{
    [JsonPropertyName("operation_type")]
    public OperationType OperationType { get; set; }
}