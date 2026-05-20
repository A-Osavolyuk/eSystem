using System.Text.Json.Serialization;

namespace eSecurity.Core.Requests;

public sealed class CheckUsernameRequest
{
    [JsonPropertyName("username")]
    public required string Username { get; set; }
}