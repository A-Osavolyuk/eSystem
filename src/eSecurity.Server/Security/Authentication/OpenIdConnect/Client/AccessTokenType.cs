using eSystem.Core.Enums;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Client;

public enum AccessTokenType
{
    [EnumValue("jwt")]
    Jwt,
    
    [EnumValue("reference")]
    Reference
}