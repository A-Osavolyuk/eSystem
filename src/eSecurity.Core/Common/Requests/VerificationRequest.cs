using System.Text.Json.Serialization;
using eSecurity.Core.Security.Authorization.Verification;

namespace eSecurity.Core.Common.Requests;

public sealed class VerificationRequest
{
    [JsonPropertyName("action")]
    public required ActionType Action { get; set; }
    
    [JsonPropertyName("purpose")]
    public required PurposeType Purpose { get; set; }
    
    [JsonPropertyName("method")] 
    public required VerificationMethod Method { get; set; }

    [JsonPropertyName("payload")]
    public required VerificationPayload Payload { get; set; }
}