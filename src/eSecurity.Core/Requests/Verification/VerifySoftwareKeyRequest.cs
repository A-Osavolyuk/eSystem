using System.Text.Json.Serialization;
using eSecurity.WebAuthN;

namespace eSecurity.Core.Requests.Verification;

public sealed class VerifySoftwareKeyRequest : VerificationRequest
{
    [JsonPropertyName("credential")]
    public required PublicKeyCredential Credential { get; set; }
}