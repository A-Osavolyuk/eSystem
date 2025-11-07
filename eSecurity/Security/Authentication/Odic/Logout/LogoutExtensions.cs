using eSecurity.Security.Authentication.Odic.Logout.Strategies;

namespace eSecurity.Security.Authentication.Odic.Logout;

public static class LogoutExtensions
{
    public static void AddLogoutFlow(this IServiceCollection services)
    {
        services.AddScoped<ILogoutStrategyResolver, LogoutStrategyResolver>();
        services.AddKeyedScoped<ILogoutStrategy, ManualLogoutStrategy>(LogoutType.Manual);
    }
}