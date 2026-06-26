using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSecurity.Idp.Security.Authentication.AuthenticationSession;

[JsonConverter(typeof(JsonEnumValueConverter<AuthenticationMethodType>))]
public enum AuthenticationMethodType
{
    [EnumValue("required")]
    Required,
    
    [EnumValue("passed")]
    Passed,
    
    [EnumValue("allowed_mfa")]
    AllowedMfa
}