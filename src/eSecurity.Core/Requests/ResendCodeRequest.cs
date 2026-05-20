using System.Text.Json.Serialization;
using eSystem.Core.Messaging;

namespace eSecurity.Core.Requests;

public sealed class ResendCodeRequest
{
    [JsonPropertyName("sender")]
    public required SenderType Sender { get; set; }
    
    [JsonPropertyName("payload")]
    public required Dictionary<string, string> Payload { get; set; }
}