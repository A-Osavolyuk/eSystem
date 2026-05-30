using System.Text.Json.Serialization;

namespace eSystem.Core.Server.Messaging.Email;

public sealed class EmailCredentials
{
    [JsonPropertyName("to")]
    public required string To { get; set; }
    
    [JsonPropertyName("subject")]
    public required string Subject { get; set; }
}