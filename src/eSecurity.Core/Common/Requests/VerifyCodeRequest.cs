namespace eSecurity.Core.Common.Requests;

public sealed class VerifyCodeRequest
{
    [JsonPropertyName("code")]
    public required string Code { get; set; }
}