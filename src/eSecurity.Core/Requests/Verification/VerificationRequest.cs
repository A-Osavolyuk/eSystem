using System.Text.Json.Serialization;
using eSecurity.Core.Security.Authorization.Verification;

namespace eSecurity.Core.Requests.Verification;

public abstract class VerificationRequest
{
    [JsonPropertyName("action")]
    public required ActionType Action { get; set; }

    [JsonPropertyName("purpose")]
    public required PurposeType Purpose { get; set; }
}