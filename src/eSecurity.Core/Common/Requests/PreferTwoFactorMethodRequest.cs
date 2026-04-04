using eSecurity.Core.Security.Authentication.TwoFactor;
using eSystem.Core.Enums;

namespace eSecurity.Core.Common.Requests;

public sealed class PreferTwoFactorMethodRequest
{
    [JsonPropertyName("preferred_method")]
    [JsonConverter(typeof(JsonEnumValueStringConverter<TwoFactorMethod>))]
    public TwoFactorMethod PreferredMethod { get; set; }
}