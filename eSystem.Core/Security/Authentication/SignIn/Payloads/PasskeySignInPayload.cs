using eSystem.Core.Security.Credentials.PublicKey;

namespace eSystem.Core.Security.Authentication.SignIn.Payloads;

public sealed class PasskeySignInPayload : SignInPayload
{
    public required PublicKeyCredential Credential { get; set; }
}