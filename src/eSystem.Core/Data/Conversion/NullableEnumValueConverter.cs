using eSystem.Core.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace eSystem.Core.Data.Conversion;

public sealed class NullableEnumValueConverter<TEnum> : ValueConverter<TEnum?, string?> 
    where TEnum : struct, Enum
{
    public NullableEnumValueConverter() 
        : base(
            v => v == null ? null : EnumHelper.GetString(v.Value), 
            v => v == null ? null : EnumHelper.FromStringOrThrow<TEnum>(v).Value) { }
}