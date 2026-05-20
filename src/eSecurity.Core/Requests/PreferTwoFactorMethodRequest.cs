using System.Text.Json.Serialization;
using eSecurity.Core.Security.Authentication.TwoFactor;

namespace eSecurity.Core.Requests;

public sealed class PreferTwoFactorMethodRequest
{
    [JsonPropertyName("preferred_method")]
    public TwoFactorMethod PreferredMethod { get; set; }
}