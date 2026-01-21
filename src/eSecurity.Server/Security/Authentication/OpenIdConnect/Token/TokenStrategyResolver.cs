namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Token;

public class TokenStrategyResolver(IServiceProvider serviceProvider) : ITokenStrategyResolver
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public ITokenStrategy Resolve(string grantType) 
        => _serviceProvider.GetRequiredKeyedService<ITokenStrategy>(grantType);
}