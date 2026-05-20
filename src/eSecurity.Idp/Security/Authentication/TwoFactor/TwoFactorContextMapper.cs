using eSecurity.Idp.Security.Authentication.TwoFactor.Authenticator;
using eSecurity.Idp.Security.Authentication.TwoFactor.Passkey;
using eSecurity.Idp.Security.Authentication.TwoFactor.RecoveryCode;
using eSecurity.Core.Security.Authentication.SignIn;
using eSecurity.Core.Security.Authentication.TwoFactor;

namespace eSecurity.Idp.Security.Authentication.TwoFactor;

public sealed class TwoFactorContextMapper : ITwoFactorContextMapper
{
    public TwoFactorContext Map(TwoFactorSignInPayload payload)
    {
        return payload.Payload switch
        {
            AuthenticatorTwoFactorPayload authenticatorPayload => new AuthenticatorTwoFactorContext()
            {
                Code = authenticatorPayload.Code, 
                TransactionId = payload.TransactionId
            },
            PasskeyTwoFactorPayload passkeyPayload => new PasskeyTwoFactorContext()
            {
                Credential = passkeyPayload.Credential, 
                TransactionId = payload.TransactionId
            },
            RecoveryCodeTwoFactorPayload recoveryCodePayload => new RecoveryCodeTwoFactorContext()
            {
                Code = recoveryCodePayload.Code, 
                TransactionId = payload.TransactionId
            },
            _ => throw new NotSupportedException("Unsupported 2FA payload type")
        };
    }
}