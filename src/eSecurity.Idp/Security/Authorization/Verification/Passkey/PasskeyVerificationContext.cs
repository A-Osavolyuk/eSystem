using eSecurity.WebAuthN;

namespace eSecurity.Idp.Security.Authorization.Verification.Passkey;

public sealed class PasskeyVerificationContext : VerificationContext
{
    public required PublicKeyCredential Credential { get; set; }
}