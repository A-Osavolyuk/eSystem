using eSystem.Core.Enums;

namespace eSecurity.Core.Security.Identity;

[JsonConverter(typeof(JsonEnumValueStringConverter<PhoneNumberType>))]
public enum PhoneNumberType
{
    [EnumValue("primary")]
    Primary,
    
    [EnumValue("secondary")]
    Secondary,
    
    [EnumValue("recovery")]
    Recovery
}