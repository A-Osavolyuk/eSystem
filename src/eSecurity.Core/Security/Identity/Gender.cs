using eSystem.Core.Enums;

namespace eSecurity.Core.Security.Identity;

public enum Gender
{
    [EnumValue("unspecified")]
    Unspecified,
    
    [EnumValue("male")]
    Male,
    
    [EnumValue("female")]
    Female,
}