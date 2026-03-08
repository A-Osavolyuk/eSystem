using eSecurity.Core.Security.Authorization.Verification;
using eSystem.Core.Common.Messaging;

namespace eSecurity.Core.Common.Requests;

public sealed class SendCodeRequest
{
    [JsonPropertyName("subject")]
    public required string Subject { get; set; }
    
    [JsonPropertyName("sender")]
    public required SenderType Sender { get; set; }
    
    [JsonPropertyName("purpose")]
    public required PurposeType Purpose { get; set; }
    
    [JsonPropertyName("action")]
    public required ActionType Action { get; set; }
    
    [JsonPropertyName("payload")]
    public required Dictionary<string, string> Payload { get; set; }
}