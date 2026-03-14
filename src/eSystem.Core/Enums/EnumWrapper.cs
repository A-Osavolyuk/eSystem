namespace eSystem.Core.Enums;

public readonly struct EnumWrapper<TEnum>(TEnum value, string alias)
    where TEnum : struct, Enum
{
    public string Alias { get; } = alias;
    public TEnum Value { get; } = value;
    
    public static implicit operator EnumWrapper<TEnum>(TEnum value)
    {
        return new EnumWrapper<TEnum>(value, EnumHelper.GetString(value));
    }
}