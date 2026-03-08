using eSecurity.Core.Security.Credentials.PublicKey;

namespace eSecurity.Core.Common.Requests;

public sealed class CreatePasskeyRequest
{
    [JsonPropertyName("display_name")]
    public required string DisplayName { get; set; }
    
    [JsonPropertyName("response")]
    public required PublicKeyCredentialCreationResponse Response { get; set; }
}