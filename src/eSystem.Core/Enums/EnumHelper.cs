using System.Reflection;
using System.Runtime.Serialization;

namespace eSystem.Core.Enums;

public static class EnumHelper
{
    public static string? GetString<TEnum>(TEnum value) where TEnum : struct, Enum
    {
        var field = typeof(TEnum).GetField(value.ToString());
        if (field is null)
            return null;

        var attribute = field.GetCustomAttribute<EnumMemberAttribute>();
        return string.IsNullOrEmpty(attribute?.Value) ? null : attribute.Value;
    }

    public static TEnum? FromString<TEnum>(string value) where TEnum : struct, Enum
    {
        var fields = typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static);
        foreach (var field in fields)
        {
            var attribute = field.GetCustomAttribute<EnumMemberAttribute>();
            if (string.IsNullOrEmpty(attribute?.Value))
                continue;
            
            if (!string.Equals(attribute.Value, value, StringComparison.Ordinal))
                continue;
            
            var enumValue = field.GetValue(null);
            if (enumValue is null)
                return null;
                
            return (TEnum)enumValue;
        }
        
        return null;
    }
}