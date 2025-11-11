namespace eSecurity.Server.Security.Authentication.Odic.Token;

public class TokenStrategyResolver(IServiceProvider serviceProvider) : ITokenStrategyResolver
{
    private readonly IServiceProvider serviceProvider = serviceProvider;

    public ITokenStrategy Resolve(string grantType) 
        => serviceProvider.GetRequiredKeyedService<ITokenStrategy>(grantType);
}