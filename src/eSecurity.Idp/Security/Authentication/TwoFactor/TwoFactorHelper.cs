using eSecurity.Core.Security.Authentication.TwoFactor;

namespace eSecurity.Idp.Security.Authentication.TwoFactor;

public static class TwoFactorHelper
{
    public static int GetMethodPriority(TwoFactorMethod method)
    {
        return method switch
        {
            TwoFactorMethod.HardwareKey => 4,
            TwoFactorMethod.SoftwareKey => 3,
            TwoFactorMethod.AuthenticatorApp => 2,
            TwoFactorMethod.EmailOtp => 1,
            TwoFactorMethod.SmsOtp => 0,
            TwoFactorMethod.RecoveryCode => 0,
            TwoFactorMethod.None => -1,
            _ => throw new NotSupportedException("Unsupported 2FA method")
        };
    }
}