using System.Text.Json.Serialization;

namespace eSystem.Core.Server.Messaging.Sms;

public sealed class SendSmsRequest
{
    [JsonPropertyName("credentials")]
    public required SmsCredentials Credentials { get; set; }
    
    [JsonPropertyName("body")]
    public required string Body { get; set; }
}