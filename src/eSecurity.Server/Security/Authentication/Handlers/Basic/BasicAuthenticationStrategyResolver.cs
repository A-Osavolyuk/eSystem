namespace eSecurity.Server.Security.Authentication.Handlers.Basic;

public class BasicAuthenticationStrategyResolver(
    IEnumerable<IBasicAuthenticationStrategy> strategies) : IBasicAuthenticationStrategyResolver
{
    private readonly IEnumerable<IBasicAuthenticationStrategy> _strategies = strategies;

    public IBasicAuthenticationStrategy Resolve(HttpContext httpContext)
    {
        foreach (var strategy in _strategies)
        {
            if (strategy.CanExecute(httpContext))
            {
                return strategy;
            }
        }
        
        throw new InvalidOperationException("No suitable basic authentication strategy found.");
    }
}