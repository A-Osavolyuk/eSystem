using System.Reflection;

namespace eSystem.Core.Enums;

public static class EnumHelper
{
    public static string GetString<TEnum>(TEnum value, bool isPreferred = true) where TEnum : struct, Enum
    {
        var aliases = EnumCache<TEnum>.To[value];
        return aliases.First(alias => alias.IsPreferred == isPreferred).Value;
    }

    public static List<string> GetStrings<TEnum>(TEnum value) where TEnum : struct, Enum
    {
        var aliases = EnumCache<TEnum>.To[value];
        return aliases.Select(alias => alias.Value).ToList();
    }

    public static TEnum? FromString<TEnum>(string value) where TEnum : struct, Enum
    {
        if (!EnumCache<TEnum>.From.TryGetValue(value, out var enumValue))
            return null;
        
        return enumValue;
    }

    public static TEnum FromStringOrThrow<TEnum>(string value) where TEnum : struct, Enum
    {
        if (!EnumCache<TEnum>.From.TryGetValue(value, out var enumValue))
            throw new NotSupportedException($"Invalid enum value key '{value}'");
        
        return enumValue;
    }
}