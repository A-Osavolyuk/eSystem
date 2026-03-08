namespace eSecurity.Core.Common.Requests;

public sealed class VerifyAuthenticatorRequest
{
    [JsonPropertyName("code")]
    public required string Code { get; set; }
    
    [JsonPropertyName("secret")]
    public required string Secret { get; set; }
}