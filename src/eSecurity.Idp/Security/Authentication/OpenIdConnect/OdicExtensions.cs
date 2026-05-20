using eSecurity.Idp.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Idp.Security.Authentication.OpenIdConnect.Logout;
using eSecurity.Idp.Security.Authentication.OpenIdConnect.Logout.Strategies;
using eSecurity.Idp.Security.Authentication.OpenIdConnect.Prompt;
using eSecurity.Idp.Security.Authentication.OpenIdConnect.Prompt.Handlers;
using eSecurity.Idp.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Idp.Security.Cryptography.Pkce;
using eSecurity.Idp.Security.Authentication.OpenIdConnect.BackchannelAuthentication;
using eSystem.Core.Primitives;
using eSystem.Core.Server.Security.Authentication.OpenIdConnect.Discovery;

namespace eSecurity.Idp.Security.Authentication.OpenIdConnect;

using SessionOptions = Session.SessionOptions;

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
            services.AddScoped<ISessionAccessor, SessionAccessor>();
            services.AddScoped<ISessionCookieFactory, SessionCookieFactory>();

            services.AddScoped<IPromptHandler, LoginPromptHandler>();
            services.AddScoped<IPromptHandler, ConsentPromptHandler>();
            services.AddScoped<IPromptHandler, SelectAccountPromptHandler>();
            services.AddScoped<IPromptHandler, NonePromptHandler>();
            
            services.AddBackchannelAuthentication(options =>
            {
                options.Interval = 5;
                options.AuthReqIdLength = 32;
                options.DefaultRequestLifetime = TimeSpan.FromSeconds(300);
                options.MinRequestLifetime = TimeSpan.FromSeconds(60);
                options.MaxRequestLifetime = TimeSpan.FromSeconds(600);
            });
            
            services.AddSession(cfg => { cfg.Timestamp = TimeSpan.FromDays(30); });
        }

        private void AddSession(Action<SessionOptions> configure)
        {
            services.AddScoped<ISessionManager, SessionManager>();
            services.Configure(configure);
        }
    }
}