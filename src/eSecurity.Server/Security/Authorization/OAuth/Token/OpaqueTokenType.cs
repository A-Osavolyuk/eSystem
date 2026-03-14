using eSystem.Core.Enums;

namespace eSecurity.Server.Security.Authorization.OAuth.Token;

public enum OpaqueTokenType
{
    [EnumValue("access_token")]
    AccessToken,
    
    [EnumValue("refresh_token")]
    RefreshToken,
    
    [EnumValue("login_token")]
    LoginToken
}