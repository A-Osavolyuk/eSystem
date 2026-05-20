namespace eSystem.Core.Enums;

public static class EnumExtensions
{
    extension<TEnum>(TEnum value) where TEnum : Enum
    {
        public string GetString() => EnumHelper.GetString(value);
    }
}