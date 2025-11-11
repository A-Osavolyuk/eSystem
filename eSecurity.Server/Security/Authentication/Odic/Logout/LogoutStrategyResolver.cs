using eSecurity.Core.Security.Authentication.Odic.Logout;

namespace eSecurity.Server.Security.Authentication.Odic.Logout;

public class LogoutStrategyResolver(IServiceProvider serviceProvider) : ILogoutStrategyResolver
{
    private readonly IServiceProvider serviceProvider = serviceProvider;

    public ILogoutStrategy Resolve(LogoutType type) => serviceProvider.GetRequiredKeyedService<ILogoutStrategy>(type);
}