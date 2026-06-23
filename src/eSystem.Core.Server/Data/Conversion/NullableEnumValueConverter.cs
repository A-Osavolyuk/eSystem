using eSystem.Core.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace eSystem.Core.Server.Data.Conversion;

public sealed class NullableEnumValueConverter<TEnum> : ValueConverter<TEnum?, string?> 
    where TEnum : struct, Enum
{
    public NullableEnumValueConverter() 
        : base(
            v => v == null ? null : GetString(v.Value), 
            v => v == null ? null : FromString(v)) { }

    private static string GetString(TEnum value)
    {
        return EnumHelper.GetString(value);
    }

    private static TEnum FromString(string value)
    {
        if (!EnumHelper.TryParseFromString<TEnum>(value, out var enumValue))
            throw new NotSupportedException($"Invalid enum value key '{value}'");

        return enumValue.Value;
    }
}