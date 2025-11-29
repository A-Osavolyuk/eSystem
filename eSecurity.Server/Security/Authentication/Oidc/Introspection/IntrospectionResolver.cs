namespace eSecurity.Server.Security.Authentication.Oidc.Introspection;

public class IntrospectionResolver(IServiceProvider serviceProvider) : IIntrospectionResolver
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public IIntrospectionStrategy Resolve(string tokenTypeHint) 
        => _serviceProvider.GetRequiredKeyedService<IIntrospectionStrategy>(tokenTypeHint);
}