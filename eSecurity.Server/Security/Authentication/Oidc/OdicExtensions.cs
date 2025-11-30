using eSecurity.Core.Security.Authentication.Oidc;
using eSecurity.Server.Security.Authentication.Oidc.Client;
using eSecurity.Server.Security.Authentication.Oidc.Code;
using eSecurity.Server.Security.Authentication.Oidc.Constants;
using eSecurity.Server.Security.Authentication.Oidc.Introspection;
using eSecurity.Server.Security.Authentication.Oidc.Pkce;
using eSecurity.Server.Security.Authentication.Oidc.Session;
using eSecurity.Server.Security.Authentication.Oidc.Token;
using eSecurity.Server.Security.Authentication.Oidc.Token.Strategies;
using eSecurity.Server.Security.Identity.Claims;
using eSystem.Core.Security.Authentication.Oidc;
using eSystem.Core.Security.Authentication.Oidc.Authorization;
using eSystem.Core.Security.Authentication.Oidc.Revocation;
using eSystem.Core.Security.Authentication.Oidc.Token;
using SessionOptions = eSecurity.Server.Security.Authentication.Oidc.Session.SessionOptions;

namespace eSecurity.Server.Security.Authentication.Oidc;

public static class OdicExtensions
{
    extension(IServiceCollection services)
    {
        public void AddOidc(Action<OpenIdOptions> configure)
        {
            services.AddScoped<IClientManager, ClientManager>();
            services.AddScoped<IAuthorizationCodeManager, AuthorizationCodeManager>();
            services.AddScoped<IPkceHandler, PkceHandler>();
            
            services.AddScoped<ITokenManager, TokenManager>();
            services.AddScoped<ITokenValidator, TokenValidator>();
            services.AddScoped<ITokenStrategyResolver, TokenStrategyResolver>();
            services.AddKeyedScoped<ITokenStrategy, AuthorizationCodeStrategy>(GrantTypes.AuthorizationCode);
            services.AddKeyedScoped<ITokenStrategy, RefreshTokenStrategy>(GrantTypes.RefreshToken);
            services.AddSingleton<IClaimBuilderFactory, ClaimBuilderFactory>();
            
            services.AddScoped<IIntrospectionResolver, IntrospectionResolver>();
            services.AddKeyedScoped<IIntrospectionStrategy, JwtTokenIntrospectionStrategy>(IntrospectionType.Jwt);
            services.AddKeyedScoped<IIntrospectionStrategy, ReferenceTokenIntrospectionStrategy>(IntrospectionType.Reference);
            
            services.AddSession(cfg => { cfg.Timestamp = TimeSpan.FromDays(30); });
        }

        private void AddSession(Action<SessionOptions> configure)
        {
            services.AddScoped<ISessionManager, SessionManager>();
            services.Configure(configure);
        }
    }
}