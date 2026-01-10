namespace eSecurity.Core.Common.Routing;

public class NavigationContext
{
    public object? Value { get; set; }
    
    public TValue? GetValue<TValue>() where TValue : class => Value as TValue;
}