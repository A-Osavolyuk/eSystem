using System.Text.Json.Serialization;
using eSecurity.Core.Security.Authentication.TwoFactor;

namespace eSecurity.Core.DTOs;

public class UserTwoFactorMethod
{
    [JsonPropertyName("method")]
    public required TwoFactorMethod Method { get; set; }
    
    [JsonPropertyName("preferred")]
    public required bool Preferred { get; set; }
    
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; set; }
}