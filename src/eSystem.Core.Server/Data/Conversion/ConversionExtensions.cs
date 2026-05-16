using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSystem.Core.Server.Data.Conversion;

public static class ConversionExtensions
{
    public static PropertyBuilder<TEnum?> HasEnumConversion<TEnum>(
        this PropertyBuilder<TEnum?> propertyBuilder)
        where TEnum : struct, Enum
    {
        return propertyBuilder.HasConversion(new NullableEnumValueConverter<TEnum>());
    }

    public static PropertyBuilder<TEnum> HasEnumConversion<TEnum>(
        this PropertyBuilder<TEnum> propertyBuilder)
        where TEnum : struct, Enum
    {
        return propertyBuilder.HasConversion(new EnumValueConverter<TEnum>());
    }
}