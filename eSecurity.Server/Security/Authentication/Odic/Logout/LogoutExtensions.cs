using eSecurity.Core.Security.Authentication.Odic.Logout;
using eSecurity.Server.Security.Authentication.Odic.Logout.Strategies;

namespace eSecurity.Server.Security.Authentication.Odic.Logout;

public static class LogoutExtensions
{
    public static void AddLogoutFlow(this IServiceCollection services)
    {
        services.AddScoped<ILogoutStrategyResolver, LogoutStrategyResolver>();
        services.AddKeyedScoped<ILogoutStrategy, ManualLogoutStrategy>(LogoutType.Manual);
    }
}