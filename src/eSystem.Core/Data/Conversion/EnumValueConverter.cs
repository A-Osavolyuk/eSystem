using eSystem.Core.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace eSystem.Core.Data.Conversion;

public sealed class EnumValueConverter<TEnum> : ValueConverter<TEnum, string> 
    where TEnum : struct, Enum
{
    public EnumValueConverter() 
        : base(
            v => EnumHelper.GetString(v), 
            v => EnumHelper.FromStringOrThrow<TEnum>(v).Value) { }
}