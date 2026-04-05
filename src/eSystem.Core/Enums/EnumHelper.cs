namespace eSystem.Core.Enums;

public static class EnumHelper
{
    public static string GetString<TEnum>(TEnum value) where TEnum : struct, Enum
    {
        var aliases = EnumCache<TEnum>.To[value];
        var alias = aliases.FirstOrDefault(alias => alias.IsPreferred);
        return alias is null ? aliases.First().Value : alias.Value;
    }
    
    public static string? GetString<TEnum>(TEnum value, bool isPreferred) where TEnum : struct, Enum
    {
        var aliases = EnumCache<TEnum>.To[value];
        var alias = aliases.FirstOrDefault(alias => alias.IsPreferred == isPreferred);
        return alias?.Value;
    }

    public static EnumValue<TEnum>? FromString<TEnum>(string? value) where TEnum : struct, Enum
    {
        return string.IsNullOrEmpty(value) 
            ? null 
            : EnumCache<TEnum>.From.GetValueOrDefault(value);
    }

    public static EnumValue<TEnum> FromStringOrThrow<TEnum>(string value) where TEnum : struct, Enum
    {
        return !EnumCache<TEnum>.From.TryGetValue(value, out var enumValue) 
            ? throw new NotSupportedException($"Invalid enum value key '{value}'") 
            : enumValue;
    }
}