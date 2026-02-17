using eSecurity.Core.Security.Authentication.TwoFactor;

namespace eSecurity.Core.Common.Requests;

public class PreferTwoFactorMethodRequest
{
    public required string Subject { get; set; }
    public TwoFactorMethod PreferredMethod { get; set; }
}