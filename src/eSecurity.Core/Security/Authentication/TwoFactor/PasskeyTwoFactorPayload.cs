using eSecurity.WebAuthN;

namespace eSecurity.Core.Security.Authentication.TwoFactor;

public sealed class PasskeyTwoFactorPayload : TwoFactorPayload
{
    public required PublicKeyCredential Credential { get; set; }
}