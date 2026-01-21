using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Code;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Pkce;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Token;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Token.Strategies;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;
using SessionOptions = eSecurity.Server.Security.Authentication.OpenIdConnect.Session.SessionOptions;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect;

public static class OdicExtensions
{
    extension(IServiceCollection services)
    {
        public void AddOpenIdConnect(Action<OpenIdConfiguration> configure)
        {
            services.Configure(configure);
            services.AddScoped<IClientManager, ClientManager>();
            services.AddScoped<IAuthorizationCodeManager, AuthorizationCodeManager>();
            services.AddScoped<IPkceHandler, PkceHandler>();
            
            services.AddScoped<ITokenManager, TokenManager>();
            services.AddScoped<ITokenValidator, TokenValidator>();
            services.AddScoped<ITokenStrategyResolver, TokenStrategyResolver>();
            services.AddKeyedScoped<ITokenStrategy, AuthorizationCodeStrategy>(GrantTypes.AuthorizationCode);
            services.AddKeyedScoped<ITokenStrategy, RefreshTokenStrategy>(GrantTypes.RefreshToken);
            
            services.AddSession(cfg => { cfg.Timestamp = TimeSpan.FromDays(30); });
        }

        private void AddSession(Action<SessionOptions> configure)
        {
            services.AddScoped<ISessionManager, SessionManager>();
            services.Configure(configure);
        }
    }
}