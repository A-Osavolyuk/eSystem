using System.Text.Json.Serialization;

namespace eSystem.Core.Server.Messaging.Email;

public sealed class SendEmailRequest
{
    [JsonPropertyName("credentials")]
    public required EmailCredentials Credentials { get; set; }

    [JsonPropertyName("body")]
    public required string Body { get; set; }
}