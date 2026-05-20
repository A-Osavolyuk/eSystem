using System.Text.Json.Serialization;
using eSecurity.WebAuthN;

namespace eSecurity.Core.Requests;

public sealed class CreatePasskeyRequest
{
    [JsonPropertyName("display_name")]
    public required string DisplayName { get; set; }
    
    [JsonPropertyName("response")]
    public required PublicKeyCredentialCreationResponse Response { get; set; }
}