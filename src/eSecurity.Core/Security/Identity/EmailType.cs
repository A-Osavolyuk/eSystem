using eSystem.Core.Enums;

namespace eSecurity.Core.Security.Identity;

public enum EmailType
{
    [EnumValue("primary")]
    Primary,
    
    [EnumValue("recovery")]
    Recovery,
    
    [EnumValue("secondary")]
    Secondary
}