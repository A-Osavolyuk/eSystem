using eSecurity.Core.Security.Authentication.TwoFactor;

namespace eSecurity.Core.Common.Requests;

public class PreferTwoFactorMethodRequest
{
    public Guid UserId { get; set; }
    public TwoFactorMethod PreferredMethod { get; set; }
}