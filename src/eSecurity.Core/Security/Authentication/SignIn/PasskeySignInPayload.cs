using System.Text.Json.Serialization;
using eSecurity.WebAuthN;

namespace eSecurity.Core.Security.Authentication.SignIn;

public sealed class PasskeySignInPayload : SignInPayload
{
    [JsonPropertyName("credential")]
    public required PublicKeyCredential Credential { get; set; }
}