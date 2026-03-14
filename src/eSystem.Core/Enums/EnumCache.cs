using System.Reflection;

namespace eSystem.Core.Enums;

public static class EnumCache<TEnum> where TEnum : struct, Enum
{
    public static readonly Dictionary<TEnum, string> To;
    public static readonly Dictionary<string, TEnum> From;

    static EnumCache()
    {
        To = new Dictionary<TEnum, string>();
        From = new Dictionary<string, TEnum>(StringComparer.OrdinalIgnoreCase);

        var type = typeof(TEnum);
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);

        foreach (var field in fields)
        {
            var enumValue = (TEnum)field.GetValue(null)!;
            var attributes = field.GetCustomAttributes<EnumValueAttribute>().ToList();
            if (attributes.Count == 0)
            {
                To[enumValue] = field.Name;
                From[field.Name] = enumValue;
                continue;
            }

            string? preferred = null;
            foreach (var attr in attributes)
            {
                if (attr.IsPreferred)
                    preferred = attr.Value;

                From[attr.Value] = enumValue;
            }

            preferred ??= attributes.First().Value;
            To[enumValue] = preferred;
        }
    }
}