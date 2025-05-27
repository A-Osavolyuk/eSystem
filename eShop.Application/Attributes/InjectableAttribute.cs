namespace eShop.Application.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class InjectableAttribute(Type type, ServiceLifetime lifetime) : Attribute
{
    public string? Key { get; set; }
    public Type Type { get; set; } = type;
    public ServiceLifetime Lifetime { get; set; } = lifetime;
}