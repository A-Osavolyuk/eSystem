using System.Text.Json.Serialization;
using eSystem.Core.Enums;

namespace eSecurity.Server.Security.Authentication.Session;

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