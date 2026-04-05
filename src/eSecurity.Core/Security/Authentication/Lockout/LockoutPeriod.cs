using eSystem.Core.Enums;

namespace eSecurity.Core.Security.Authentication.Lockout;

[JsonConverter(typeof(JsonEnumValueStringConverter<LockoutPeriod>))]
public enum LockoutPeriod
{
    [EnumValue("day")]
    Day,
    
    [EnumValue("week")]
    Week,
    
    [EnumValue("month")]
    Month,
    
    [EnumValue("quarter")]
    Quarter,
    
    [EnumValue("year")]
    Year
}