using eSystem.Core.Security.Authorization.OAuth;

namespace eSecurity.Server.Security.Authorization.OAuth.Token;

public class TokenStrategyResolver(IServiceProvider serviceProvider) : ITokenStrategyResolver
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public ITokenStrategy Resolve(GrantType grantType) 
        => _serviceProvider.GetRequiredKeyedService<ITokenStrategy>(grantType);
}