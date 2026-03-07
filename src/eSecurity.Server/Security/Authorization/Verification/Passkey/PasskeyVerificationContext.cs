using eSecurity.Core.Security.Credentials.PublicKey;

namespace eSecurity.Server.Security.Authorization.Verification.Passkey;

public sealed class PasskeyVerificationContext : VerificationContext
{
    public required PublicKeyCredential Credential { get; set; }
}