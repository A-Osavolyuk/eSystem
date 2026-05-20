namespace eSecurity.Idp.Security.Authentication.OpenIdConnect.Logout;

public class LogoutStrategyResolver(IServiceProvider serviceProvider) : ILogoutStrategyResolver
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public ILogoutStrategy<TResult> Resolve<TResult>(LogoutFlow flow)
        => _serviceProvider.GetRequiredKeyedService<ILogoutStrategy<TResult>>(flow);
}