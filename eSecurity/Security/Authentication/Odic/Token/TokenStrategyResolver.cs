namespace eSecurity.Security.Authentication.Odic.Token;

public class TokenStrategyResolver(IServiceProvider serviceProvider) : ITokenStrategyResolver
{
    private readonly IServiceProvider serviceProvider = serviceProvider;

    public TokenStrategy Resolve(string grantType) 
        => serviceProvider.GetRequiredKeyedService<TokenStrategy>(grantType);
}