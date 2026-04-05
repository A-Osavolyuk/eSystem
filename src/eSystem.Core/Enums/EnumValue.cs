namespace eSystem.Core.Enums;

public sealed class EnumValue<TEnum>(TEnum value, bool isPreferred = false) where TEnum : struct, Enum
{
    public TEnum Value { get; init; } = value;
    public bool IsPreferred { get; init; } = isPreferred;
}