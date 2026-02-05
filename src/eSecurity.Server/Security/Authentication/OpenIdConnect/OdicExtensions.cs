using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Logout;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Logout.Strategies;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Server.Security.Cryptography.Pkce;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authentication.OpenIdConnect.Discovery;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect;

public static class OdicExtensions
{
    extension(IServiceCollection services)
    {
        public void AddOpenIdConnect(Action<OpenIdConfiguration> configure)
        {
            services.Configure(configure);
            services.AddScoped<IPkceHandler, PkceHandler>();
            services.AddScoped<IClientManager, ClientManager>();
            services.AddKeyedScoped<ILogoutStrategy<Result>, BackchannelLogoutStrategy>(LogoutFlow.Backchannel);
            services.AddKeyedScoped<ILogoutStrategy<List<string>>, FrontchannelLogoutStrategy>(LogoutFlow.Frontchannel);
            services.AddScoped<ILogoutStrategyResolver, LogoutStrategyResolver>();
            services.AddScoped<ILogoutHandler, LogoutHandler>();
            
            services.AddSession(cfg => { cfg.Timestamp = TimeSpan.FromDays(30); });
        }

        private void AddSession(Action<SessionOptions> configure)
        {
            services.AddScoped<ISessionManager, SessionManager>();
            services.Configure(configure);
        }
    }
}