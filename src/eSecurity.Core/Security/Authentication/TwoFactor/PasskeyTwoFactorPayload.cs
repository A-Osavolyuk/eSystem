using System.Text.Json.Serialization;
using eSecurity.WebAuthN;

namespace eSecurity.Core.Security.Authentication.TwoFactor;

public sealed class PasskeyTwoFactorPayload : TwoFactorPayload
{
    [JsonPropertyName("credential")]
    public required PublicKeyCredential Credential { get; set; }
}