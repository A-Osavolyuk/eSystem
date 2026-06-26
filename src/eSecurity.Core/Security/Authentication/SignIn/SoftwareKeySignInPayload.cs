using System.Text.Json.Serialization;
using eSecurity.WebAuthN;

namespace eSecurity.Core.Security.Authentication.SignIn;

public sealed class SoftwareKeySignInPayload : SignInPayload
{
    [JsonPropertyName("credential")] 
    public PublicKeyCredential? Credential { get; set; } = null!;
}