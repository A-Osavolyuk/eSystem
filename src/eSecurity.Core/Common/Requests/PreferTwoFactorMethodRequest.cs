using eSecurity.Core.Security.Authentication.TwoFactor;

namespace eSecurity.Core.Common.Requests;

public sealed class PreferTwoFactorMethodRequest
{
    [JsonPropertyName("preferred_method")]
    public TwoFactorMethod PreferredMethod { get; set; }
}