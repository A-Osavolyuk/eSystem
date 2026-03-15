using eSystem.Core.Enums;

namespace eSecurity.Server.Security.Authentication.Session;

public enum AuthenticationMethodType
{
    [EnumValue("required")]
    Required,
    
    [EnumValue("passed")]
    Passed,
    
    [EnumValue("allowed_mfa")]
    AllowedMfa
}