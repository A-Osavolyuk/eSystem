using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Code;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Constants;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Pkce;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Token;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Token.Strategies;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Token.Validation;
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

            services.AddScoped<ITokenValidationProvider, TokenValidationProvider>();
            services.AddScoped<IJwtTokenValidationProvider, JwtTokenValidationProvider>();
            services.AddKeyedScoped<ITokenValidator, OpaqueTokenValidator>(TokenTypes.Opaque);
            services.AddKeyedScoped<ITokenValidator, JwtTokenValidator>(TokenTypes.Jwt);
            services.AddKeyedScoped<IJwtTokenValidator, IdTokenValidator>(JwtTokenTypes.IdToken);
            services.AddKeyedScoped<IJwtTokenValidator, AccessTokenValidator>(JwtTokenTypes.AccessToken);
            services.AddKeyedScoped<IJwtTokenValidator, GenericJwtTokenValidator>(JwtTokenTypes.Generic);
            
            services.AddScoped<ITokenManager, TokenManager>();
            services.AddScoped<ITokenStrategyResolver, TokenStrategyResolver>();
            services.AddKeyedScoped<ITokenStrategy, AuthorizationCodeStrategy>(GrantTypes.AuthorizationCode);
            services.AddKeyedScoped<ITokenStrategy, RefreshTokenStrategy>(GrantTypes.RefreshToken);
            services.AddKeyedScoped<ITokenStrategy, ClientCredentialsStrategy>(GrantTypes.ClientCredentials);
            services.AddScoped<ITokenContextFactory, TokenContextFactory>();
            
            services.AddSession(cfg => { cfg.Timestamp = TimeSpan.FromDays(30); });
        }

        private void AddSession(Action<SessionOptions> configure)
        {
            services.AddScoped<ISessionManager, SessionManager>();
            services.Configure(configure);
        }
    }
}