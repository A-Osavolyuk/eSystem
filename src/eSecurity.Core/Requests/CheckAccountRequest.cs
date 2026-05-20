using System.Text.Json.Serialization;

namespace eSecurity.Core.Requests;

public sealed class CheckAccountRequest
{
    [JsonPropertyName("login")]
    public required string Login { get; set; }
}