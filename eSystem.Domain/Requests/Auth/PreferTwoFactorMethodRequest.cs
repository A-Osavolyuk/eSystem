using eSystem.Domain.Security.Authentication.TwoFactor;

namespace eSystem.Domain.Requests.Auth;

public class PreferTwoFactorMethodRequest
{
    public Guid UserId { get; set; }
    public TwoFactorMethod PreferredMethod { get; set; }
}