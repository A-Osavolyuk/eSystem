using eSecurity.Core.Security.Authorization.Verification;

namespace eSecurity.Core.Common.Requests;

public sealed class VerifyAuthenticatorCodeRequest
{
    [JsonPropertyName("action")]
    public required ActionType Action { get; set; }
    
    [JsonPropertyName("purpose")]
    public required PurposeType Purpose { get; set; }
    
    [JsonPropertyName("code")]
    public required string Code { get; set; }
}