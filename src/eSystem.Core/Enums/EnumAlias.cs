namespace eSystem.Core.Enums;

public readonly struct EnumAlias(string value, bool isPreferred = true)
{
    public string Value { get; } = value;
    public bool IsPreferred { get; } = isPreferred;
}