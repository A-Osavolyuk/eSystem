using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSecurity.Core.Security.Authorization.OAuth;

[JsonConverter(typeof(JsonEnumValueConverter<LinkedAccountType>))]
public enum LinkedAccountType
{
    [EnumValue("none")]
    None,
    
    [EnumValue("google")]
    Google,
    
    [EnumValue("microsoft")]
    Microsoft,
    
    [EnumValue("facebook")]
    Facebook,
    
    [EnumValue("x")]
    X
}