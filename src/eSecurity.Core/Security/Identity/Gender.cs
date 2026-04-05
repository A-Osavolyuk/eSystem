using eSystem.Core.Enums;

namespace eSecurity.Core.Security.Identity;

[JsonConverter(typeof(JsonEnumValueStringConverter<Gender>))]
public enum Gender
{
    [EnumValue("unspecified")]
    Unspecified,
    
    [EnumValue("male")]
    Male,
    
    [EnumValue("female")]
    Female,
}