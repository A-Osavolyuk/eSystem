using eSecurity.Core.Security.Authentication.Odic.Logout;
using eSecurity.Server.Security.Authentication.Odic.Logout.Strategies;

namespace eSecurity.Server.Security.Authentication.Odic.Logout;

public static class LogoutExtensions
{
    extension(IServiceCollection services)
    {
        public void AddLogoutFlow()
        {
            services.AddScoped<ILogoutStrategyResolver, LogoutStrategyResolver>();
            services.AddKeyedScoped<ILogoutStrategy, ManualLogoutStrategy>(LogoutType.Manual);
            services.AddKeyedScoped<ILogoutStrategy, OidcLogoutStrategy>(LogoutType.Oidc);
        }
    }
}