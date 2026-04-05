using eSystem.Core.Enums;

namespace eSecurity.Core.Security.Authorization.OAuth;

[JsonConverter(typeof(JsonEnumValueConverter<LinkedAccountType>))]
public enum LinkedAccountType
{
    [EnumValue("google")]
    Google,
    
    [EnumValue("microsoft")]
    Microsoft,
    
    [EnumValue("facebook")]
    Facebook,
    
    [EnumValue("x")]
    X
}