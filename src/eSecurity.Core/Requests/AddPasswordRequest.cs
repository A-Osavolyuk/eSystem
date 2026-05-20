using System.Text.Json.Serialization;

namespace eSecurity.Core.Requests;

public sealed class AddPasswordRequest
{
    [JsonPropertyName("password")]
    public required string Password { get; set; }
}