namespace eSecurity.Server.Security.Authentication.Oidc.Introspection;

public class IntrospectionResolver(IServiceProvider serviceProvider) : IIntrospectionResolver
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public IIntrospectionStrategy Resolve(IntrospectionType type) 
        => _serviceProvider.GetRequiredKeyedService<IIntrospectionStrategy>(type);
}