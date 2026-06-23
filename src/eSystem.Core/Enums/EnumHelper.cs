namespace eSystem.Core.Enums;

public static class EnumHelper
{
    public static string GetString<TEnum>(TEnum value) where TEnum : Enum
    {
        ArgumentNullException.ThrowIfNull(value);
        
        var aliases = EnumCache<TEnum>.To[value];
        var alias = aliases.FirstOrDefault(alias => alias.IsPreferred);
        return alias is null ? aliases.First().Value : alias.Value;
    }
    
    public static string? GetString<TEnum>(TEnum value, bool isPreferred) where TEnum : Enum
    {
        ArgumentNullException.ThrowIfNull(value);
        
        var aliases = EnumCache<TEnum>.To[value];
        var alias = aliases.FirstOrDefault(alias => alias.IsPreferred == isPreferred);
        return alias?.Value;
    }

    public static EnumValue<TEnum>? ParseFromString<TEnum>(string value) where TEnum : Enum
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return string.IsNullOrEmpty(value) ? null : EnumCache<TEnum>.From.GetValueOrDefault(value);
    }

    public static bool TryParseFromString<TEnum>(string value, out EnumValue<TEnum> enumValue) where TEnum : Enum
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        var cachedValue = EnumCache<TEnum>.From.GetValueOrDefault(value);
        if (cachedValue is null)
        {
            enumValue = null!;
            return false;
        }

        enumValue = cachedValue;
        return true;
    }
}