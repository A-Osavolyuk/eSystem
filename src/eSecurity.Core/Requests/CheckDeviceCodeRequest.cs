using System.Text.Json.Serialization;

namespace eSecurity.Core.Requests;

public sealed class CheckDeviceCodeRequest
{
    [JsonPropertyName("user_code")]
    public required string UserCode { get; set; }
}