using eSecurity.Idp.Security.Authentication.TwoFactor.RecoveryCode;
using eSecurity.Core.Security.Authentication.SignIn;
using eSecurity.Core.Security.Authentication.TwoFactor;
using eSecurity.Idp.Security.Authentication.TwoFactor.AuthenticatorApp;
using eSecurity.Idp.Security.Authentication.TwoFactor.SoftwareKey;

namespace eSecurity.Idp.Security.Authentication.TwoFactor;

public sealed class TwoFactorContextMapper : ITwoFactorContextMapper
{
    public TwoFactorContext Map(TwoFactorSignInPayload payload)
    {
        return payload.Payload switch
        {
            AuthenticatorTwoFactorPayload authenticatorPayload => new AuthenticatorTwoFactorContext
            {
                Code = authenticatorPayload.Code, 
                TransactionId = payload.TransactionId
            },
            SoftwareKeyTwoFactorPayload passkeyPayload => new SoftwareKeyTwoFactorContext
            {
                Credential = passkeyPayload.Credential, 
                TransactionId = payload.TransactionId
            },
            RecoveryCodeTwoFactorPayload recoveryCodePayload => new RecoveryCodeTwoFactorContext
            {
                Code = recoveryCodePayload.Code, 
                TransactionId = payload.TransactionId
            },
            _ => throw new NotSupportedException("Unsupported 2FA payload type")
        };
    }
}