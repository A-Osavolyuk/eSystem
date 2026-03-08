using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Core.Security.Credentials.PublicKey;

namespace eSecurity.Core.Common.Requests;

public sealed class VerifyPasskeyRequest
{
    [JsonPropertyName("credential")]
    public required PublicKeyCredential Credential { get; set; }
    
    [JsonPropertyName("purpose")]
    public required PurposeType Purpose { get; set; }
    
    [JsonPropertyName("action")]
    public required ActionType Action { get; set; }
}