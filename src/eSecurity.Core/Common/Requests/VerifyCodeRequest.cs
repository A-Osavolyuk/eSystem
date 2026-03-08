namespace eSecurity.Core.Common.Requests;

public sealed class VerifyCodeRequest
{
    [JsonPropertyName("subject")]
    public required string Subject { get; set; }
    
    [JsonPropertyName("code")]
    public required string Code { get; set; }
}