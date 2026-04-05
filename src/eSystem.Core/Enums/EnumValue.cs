namespace eSystem.Core.Enums;

public record EnumValue<TEnum>(TEnum Value, bool IsPreferred = false) where TEnum : struct, Enum;