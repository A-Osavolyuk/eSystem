using Microsoft.Extensions.DependencyInjection;

namespace eSystem.Core.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class InjectableAttribute(Type type, ServiceLifetime lifetime) : Attribute
{
    public Type Type { get; } = type;
    public ServiceLifetime Lifetime { get; } = lifetime;
    public string? Key { get; set; }
}