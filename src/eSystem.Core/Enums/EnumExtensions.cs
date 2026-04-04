namespace eSystem.Core.Enums;

public static class EnumExtensions
{
    extension<TEnum>(TEnum value) where TEnum : struct, Enum
    {
        public string GetString() => EnumHelper.GetString(value);
    }
}