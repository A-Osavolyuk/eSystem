using eSecurity.Core.Security.Authentication.TwoFactor;

namespace eSecurity.Core.Common.DTOs;

public class UserTwoFactorMethod
{
    [JsonPropertyName("method")]
    public required TwoFactorMethod Method { get; set; }
    
    [JsonPropertyName("preferred")]
    public required bool Preferred { get; set; }
    
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; set; }
}