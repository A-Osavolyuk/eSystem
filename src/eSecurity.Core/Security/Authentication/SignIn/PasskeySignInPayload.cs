using eSecurity.WebAuthN;

namespace eSecurity.Core.Security.Authentication.SignIn;

public sealed class PasskeySignInPayload : SignInPayload
{
    public required PublicKeyCredential Credential { get; set; }
}