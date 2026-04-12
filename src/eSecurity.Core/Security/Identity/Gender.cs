using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSecurity.Core.Security.Identity;

[JsonConverter(typeof(JsonEnumValueConverter<Gender>))]
public enum Gender
{
    [EnumValue("unspecified")]
    Unspecified,
    
    [EnumValue("male")]
    Male,
    
    [EnumValue("female")]
    Female,
}