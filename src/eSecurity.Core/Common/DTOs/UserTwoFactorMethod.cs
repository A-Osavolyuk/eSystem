using eSecurity.Core.Security.Authentication.TwoFactor;
using eSystem.Core.Enums;

namespace eSecurity.Core.Common.DTOs;

public class UserTwoFactorMethod
{
    [JsonPropertyName("method")]
    [JsonConverter(typeof(JsonEnumValueStringConverter<TwoFactorMethod>))]
    public required TwoFactorMethod Method { get; set; }
    
    [JsonPropertyName("preferred")]
    public required bool Preferred { get; set; }
    
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; set; }
}