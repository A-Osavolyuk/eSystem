using eSecurity.Core.Security.Authentication.Odic.Logout;

namespace eSecurity.Server.Security.Authentication.Odic.Logout;

public class LogoutStrategyResolver(IServiceProvider serviceProvider) : ILogoutStrategyResolver
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public ILogoutStrategy Resolve(LogoutType type) => _serviceProvider.GetRequiredKeyedService<ILogoutStrategy>(type);
}