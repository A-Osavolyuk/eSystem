using eSecurity.Core.Security.Authentication.SignIn;

namespace eSecurity.Idp.Security.Authentication.TwoFactor;

public interface ITwoFactorContextMapper
{
    public TwoFactorContext Map(TwoFactorSignInPayload payload);
}