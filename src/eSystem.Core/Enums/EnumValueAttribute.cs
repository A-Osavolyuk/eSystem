namespace eSystem.Core.Enums;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public sealed class EnumValueAttribute(string value, bool isPreferred = false) : Attribute
{
    public string Value { get; } = value;
    public bool IsPreferred { get; set; } = isPreferred;
}