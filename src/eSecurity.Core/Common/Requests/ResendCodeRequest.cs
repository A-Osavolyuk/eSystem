using eSystem.Core.Common.Messaging;
using eSystem.Core.Enums;

namespace eSecurity.Core.Common.Requests;

public sealed class ResendCodeRequest
{
    [JsonPropertyName("sender")]
    [JsonConverter(typeof(JsonEnumValueStringConverter<SenderType>))]
    public required SenderType Sender { get; set; }
    
    [JsonPropertyName("payload")]
    public required Dictionary<string, string> Payload { get; set; }
}