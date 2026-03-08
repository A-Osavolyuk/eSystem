namespace eSecurity.Core.Common.Requests;

public sealed class CheckDeviceCodeRequest
{
    [JsonPropertyName("user_code")]
    public required string UserCode { get; set; }
}