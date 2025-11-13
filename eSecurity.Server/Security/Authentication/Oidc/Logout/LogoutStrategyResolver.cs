using eSecurity.Core.Security.Authentication.Oidc.Logout;

namespace eSecurity.Server.Security.Authentication.Oidc.Logout;

public class LogoutStrategyResolver(IServiceProvider serviceProvider) : ILogoutStrategyResolver
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public ILogoutStrategy Resolve(LogoutType type) => _serviceProvider.GetRequiredKeyedService<ILogoutStrategy>(type);
}