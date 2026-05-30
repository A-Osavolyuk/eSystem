using System.Text.Json.Serialization;

namespace eSystem.Core.Server.Messaging.Sms;

public sealed class SmsCredentials
{
    [JsonPropertyName("to")]
    public required string To { get; set; }
}