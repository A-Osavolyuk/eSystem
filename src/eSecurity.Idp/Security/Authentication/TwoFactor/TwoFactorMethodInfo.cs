using eSecurity.Core.Security.Authentication.TwoFactor;

namespace eSecurity.Idp.Security.Authentication.TwoFactor;

public sealed class TwoFactorMethodInfo(TwoFactorMethod method, bool isPreferred, int priority)
{
    public TwoFactorMethod Method { get; init; } = method;
    public bool IsPreferred { get; init; } = isPreferred;
    public int Priority { get; init; } = priority;
}