namespace eSystem.Core.Enums;

public static class EnumExtensions
{
    extension<TEnum>(TEnum value) where TEnum : struct, Enum
    {
        public string GetString(bool usePreferred = false) => EnumHelper.GetString(value, usePreferred);
    }
}