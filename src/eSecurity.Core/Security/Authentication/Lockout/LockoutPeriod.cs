using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSecurity.Core.Security.Authentication.Lockout;

[JsonConverter(typeof(JsonEnumValueConverter<LockoutPeriod>))]
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