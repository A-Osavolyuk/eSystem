namespace eShop.Application.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class InjectableAttribute(Type type, ServiceLifetime lifetime) : Attribute
{
    public Type Type { get; } = type;
    public ServiceLifetime Lifetime { get; } = lifetime;
    public string? Key { get; set; }
    public bool WithHttpClient { get; set; }
}