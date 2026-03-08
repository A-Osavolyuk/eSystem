using eSecurity.Core.Security.Authorization.Verification;
using eSystem.Core.Common.Messaging;

namespace eSecurity.Core.Common.Requests;

public sealed class ResendCodeRequest
{
    [JsonPropertyName("sender")]
    public required SenderType Sender { get; set; }
    
    [JsonPropertyName("payload")]
    public required Dictionary<string, string> Payload { get; set; }
}