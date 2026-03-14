using eSystem.Core.Enums;

namespace eSystem.Core.Security.Authentication.OpenIdConnect.Client;

public enum ClientType
{
    [EnumValue("confidential")]
    Confidential,
    
    [EnumValue("public")]
    Public
}