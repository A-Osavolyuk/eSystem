using eSecurity.Core.Security.Authentication.TwoFactor;

namespace eSecurity.Core.Common.Requests;

public sealed class PreferTwoFactorMethodRequest
{
    public TwoFactorMethod PreferredMethod { get; set; }
}