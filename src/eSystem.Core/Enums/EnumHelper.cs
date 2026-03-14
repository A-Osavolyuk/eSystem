using System.Reflection;

namespace eSystem.Core.Enums;

public static class EnumHelper
{
    public static string GetString<TEnum>(TEnum value) where TEnum : struct, Enum
    {
        var field = typeof(TEnum).GetField(value.ToString());
        if (field is null)
            return value.ToString();
        
        var attribute = field.GetCustomAttribute<EnumValueAttribute>();
        if (attribute is null || string.IsNullOrEmpty(attribute.Value)) 
            return field.Name;
        
        return attribute.Value;
    }

    public static TEnum? FromString<TEnum>(string value) where TEnum : struct, Enum
    {
        var fields = typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static);
        foreach (var field in fields)
        {
            var attribute = field.GetCustomAttribute<EnumValueAttribute>();
            if (attribute is null || string.IsNullOrEmpty(attribute.Value))
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