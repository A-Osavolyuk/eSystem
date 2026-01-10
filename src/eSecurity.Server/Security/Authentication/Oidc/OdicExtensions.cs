using eSecurity.Core.Security.Authentication.Oidc;
using eSecurity.Server.Security.Authentication.Oidc.Client;
using eSecurity.Server.Security.Authentication.Oidc.Code;
using eSecurity.Server.Security.Authentication.Oidc.Pkce;
using eSecurity.Server.Security.Authentication.Oidc.Session;
using eSecurity.Server.Security.Authentication.Oidc.Token;
using eSecurity.Server.Security.Authentication.Oidc.Token.Strategies;
using eSystem.Core.Security.Authentication.Oidc.Token;
using SessionOptions = eSecurity.Server.Security.Authentication.Oidc.Session.SessionOptions;

namespace eSecurity.Server.Security.Authentication.Oidc;

public static class OdicExtensions
{
    extension(IServiceCollection services)
    {
        public void AddOidc(Action<OpenIdOptions> configure)
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