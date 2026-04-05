using eSystem.Core.Enums;

namespace eSecurity.Core.Security.Identity;

[JsonConverter(typeof(JsonEnumValueConverter<EmailType>))]
public enum EmailType
{
    [EnumValue("primary")]
    Primary,
    
    [EnumValue("recovery")]
    Recovery,
    
    [EnumValue("secondary")]
    Secondary
}