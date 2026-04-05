using System.Reflection;

namespace eSystem.Core.Enums;

public static class EnumCache<TEnum> where TEnum : struct, Enum
{
    public static readonly Dictionary<TEnum, List<EnumAlias>> To;
    public static readonly Dictionary<string, EnumValue<TEnum>> From;

    static EnumCache()
    {
        To = new Dictionary<TEnum, List<EnumAlias>>();
        From = new Dictionary<string, EnumValue<TEnum>>(StringComparer.OrdinalIgnoreCase);

        var type = typeof(TEnum);
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);

        foreach (var field in fields)
        {
            var enumValue = (TEnum)field.GetValue(null)!;
            var attributes = field.GetCustomAttributes<EnumValueAttribute>().ToList();
            if (attributes.Count == 0)
            {
                To[enumValue] = [new EnumAlias(enumValue.ToString())];
                From[field.Name] = new EnumValue<TEnum>(enumValue);
                continue;
            }
            
            foreach (var attribute in attributes)
            {
                if (To.TryGetValue(enumValue, out var aliases))
                    aliases.Add(new EnumAlias(attribute.Value, attribute.IsPreferred));
                else
                    To[enumValue] = [new EnumAlias(attribute.Value)];
                
                From[attribute.Value] = new EnumValue<TEnum>(enumValue, attribute.IsPreferred);
            }
        }
    }
}