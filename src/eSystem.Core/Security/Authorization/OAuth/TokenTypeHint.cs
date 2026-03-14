using eSystem.Core.Enums;

namespace eSystem.Core.Security.Authorization.OAuth;

public enum TokenTypeHint
{
    [EnumValue("access_token")]
    AccessToken,
    
    [EnumValue("refresh_token")]
    RefreshToken
}