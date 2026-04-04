using eSecurity.Core.Security.Authorization.Verification;
using eSystem.Core.Enums;

namespace eSecurity.Core.Common.Requests;

public sealed class VerificationRequest
{
    [JsonPropertyName("action")]
    [JsonConverter(typeof(JsonEnumValueStringConverter<ActionType>))]
    public required ActionType Action { get; set; }
    
    [JsonPropertyName("purpose")]
    [JsonConverter(typeof(JsonEnumValueStringConverter<PurposeType>))]
    public required PurposeType Purpose { get; set; }
    
    [JsonPropertyName("method")] 
    [JsonConverter(typeof(JsonEnumValueStringConverter<VerificationMethod>))]
    public required VerificationMethod Method { get; set; }

    [JsonPropertyName("payload")]
    public required VerificationPayload Payload { get; set; }
}