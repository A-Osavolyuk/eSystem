namespace eSystem.Core.Enums;

public sealed class EnumAlias(string value, bool isPreferred = false)
{
    public string Value { get; init; } = value;
    public bool IsPreferred { get; init; } = isPreferred;
}