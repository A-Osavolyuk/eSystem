using eSecurity.Idp.Security.Authentication.Session;
using eSecurity.Idp.Security.Cryptography.Pkce;
using eSystem.Core.Primitives;
using eSystem.Core.Server.Security.Authentication.OpenIdConnect.Discovery;

namespace eSecurity.Idp.Security.Authentication.OpenIdConnect;

public static class OdicExtensions
{
    extension(IServiceCollection services)
    {
        public void AddOpenIdConnect(Action<OpenIdConfiguration> configure)
        {
            services.Configure(configure);
        }

        public void AddSsoSession(Action<SessionOptions> configure)
        {
            services.Configure(configure);
            services.AddScoped<ISessionManager, SessionManager>();
            services.AddScoped<ISessionAccessor, SessionAccessor>();
            services.AddScoped<ISessionCookieFactory, SessionCookieFactory>();
        }
    }
}