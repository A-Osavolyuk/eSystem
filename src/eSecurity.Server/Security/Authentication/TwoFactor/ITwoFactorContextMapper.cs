using eSecurity.Core.Security.Authentication.SignIn;

namespace eSecurity.Server.Security.Authentication.TwoFactor;

public interface ITwoFactorContextMapper
{
    public TwoFactorContext Map(TwoFactorSignInPayload payload);
}