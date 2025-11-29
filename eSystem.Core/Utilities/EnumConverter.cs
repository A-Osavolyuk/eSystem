using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;

namespace eSystem.Core.Utilities;

public static class EnumConverter
{
    public static string ToString<TEnum>(TEnum value) where TEnum : struct, Enum
    {
        var enumType = typeof(TEnum);
        var field = enumType.GetField(value.ToString(), BindingFlags.Public | BindingFlags.Static);
        var enumMember = field?.GetCustomAttribute<EnumMemberAttribute>();
        return enumMember?.Value ?? value.ToString();
    }
    
    public static TEnum FromString<TEnum>(string value) where TEnum : struct, Enum
    {
        var enumType = typeof(TEnum);
        foreach (var field in enumType.GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var enumMember = field.GetCustomAttribute<EnumMemberAttribute>();
            if (enumMember?.Value == value)
                return (TEnum)field.GetValue(null)!;
        }
        
        return Enum.TryParse<TEnum>(value, ignoreCase: true, out var enumValue) 
            ? enumValue 
            : throw new ArgumentException($"Unknown value '{value}' for enum '{enumType.Name}'.");
    }
}