using eSecurity.Core.Security.Authentication.TwoFactor;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authentication.TwoFactor;

public interface ITwoFactorPolicy
{
    Result CanSetPreferredMethod(IReadOnlyCollection<TwoFactorMethodInfo> methods, TwoFactorMethodInfo method);
    
    Result CanAddMethod(IReadOnlyCollection<TwoFactorMethodInfo> methods, TwoFactorMethod method);
}