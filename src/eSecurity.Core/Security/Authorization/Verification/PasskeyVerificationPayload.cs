using System.Text.Json.Serialization;
using eSecurity.WebAuthN;

namespace eSecurity.Core.Security.Authorization.Verification;

public sealed class PasskeyVerificationPayload : VerificationPayload
{
    [JsonPropertyName("credential")]
    public required PublicKeyCredential Credential { get; set; }
}