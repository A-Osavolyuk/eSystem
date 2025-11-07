namespace eSecurity.Security.Authentication.Odic.Logout;

public class LogoutStrategyResolver(IServiceProvider serviceProvider) : ILogoutStrategyResolver
{
    private readonly IServiceProvider serviceProvider = serviceProvider;

    public ILogoutStrategy Resolve(LogoutType type) => serviceProvider.GetRequiredKeyedService<ILogoutStrategy>(type);
}