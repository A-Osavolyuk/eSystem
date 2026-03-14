using eSystem.Core.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace eSecurity.Server.Data.Conversion;

public sealed class EnumValueConverter<TEnum> : ValueConverter<TEnum, string> 
    where TEnum : struct, Enum
{
    private static readonly Dictionary<TEnum, string> To;
    private static readonly Dictionary<string, TEnum> From;
    
    static EnumValueConverter()
    {
        To = new Dictionary<TEnum, string>();
        From = new Dictionary<string, TEnum>(StringComparer.OrdinalIgnoreCase);

        var enumValues = Enum.GetValues<TEnum>();
        foreach (var enumValue in enumValues)
        {
            var stringValue = EnumHelper.GetString(enumValue);
            To[enumValue] = stringValue;
            From[stringValue] = enumValue;
        }
    }
    
    public EnumValueConverter() 
        : base(
            v => To[v], 
            v => From[v]) { }
}