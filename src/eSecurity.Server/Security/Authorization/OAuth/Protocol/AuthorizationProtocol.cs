using eSystem.Core.Enums;

namespace eSecurity.Server.Security.Authorization.OAuth.Protocol;

public enum AuthorizationProtocol
{
    [EnumValue("openid_connect")]
    OpenIdConnect,
    
    [EnumValue("oauth")]
    OAuth
}