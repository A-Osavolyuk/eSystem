using eSystem.Core.Security.Authentication.TwoFactor;

namespace eSystem.Core.Requests.Auth;

public class PreferTwoFactorMethodRequest
{
    public Guid UserId { get; set; }
    public TwoFactorMethod PreferredMethod { get; set; }
}