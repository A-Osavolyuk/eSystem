using eSecurity.Core.Security.Authentication.Odic.Logout;
using eSecurity.Server.Security.Authentication.Odic.Client;
using eSecurity.Server.Security.Authentication.Odic.Code;
using eSecurity.Server.Security.Authentication.Odic.Configuration;
using eSecurity.Server.Security.Authentication.Odic.Logout;
using eSecurity.Server.Security.Authentication.Odic.Logout.Strategies;
using eSecurity.Server.Security.Authentication.Odic.Pkce;
using eSecurity.Server.Security.Authentication.Odic.Session;
using eSecurity.Server.Security.Authentication.Odic.Token;
using eSecurity.Server.Security.Authentication.Odic.Token.Strategies;
using eSecurity.Server.Security.Identity.Claims;
using eSystem.Core.Security.Authentication.Odic.Constants;
using SessionOptions = eSecurity.Server.Security.Authentication.Odic.Session.SessionOptions;

namespace eSecurity.Server.Security.Authentication.Odic;

public static class OdicExtensions
{
    extension(IServiceCollection services)
    {
        public void AddOdic()
        {
            services.AddPkceHandler();
            services.AddLogoutFlow();
            services.AddTokenFlow();
            services.AddClientManagement();
            services.AddAuthorizationCodeManagement();
            services.AddSession(cfg => { cfg.Timestamp = TimeSpan.FromDays(30); });
            
            services.AddOpenidConfiguration(cfg =>
            {
                cfg.Issuer = "http://localhost:5201";

                cfg.AuthorizationEndpoint = "http://localhost:5501/connect/authorize";
                cfg.EndSessionEndpoint = "http://localhost:5501/connect/logout";
                cfg.TokenEndpoint = "http://localhost:5201/api/v1/connect/token";
                cfg.UserinfoEndpoint = "http://localhost:5201/api/v1/connect/userinfo";
                cfg.IntrospectionEndpoint = "http://localhost:5201/api/v1/connect/introspect";
                cfg.RevocationEndpoint = "http://localhost:5201/api/v1/connect/revoke";
                cfg.JwksUri = "http://localhost:5201/api/v1/connect/jwks.json";

                cfg.ResponseTypesSupported = [ResponseTypes.Code, ResponseTypes.IdToken, ResponseTypes.Token];
                cfg.GrantTypesSupported = [GrantTypes.AuthorizationCode, GrantTypes.RefreshToken];

                cfg.SubjectTypesSupported = [SubjectTypes.Public, SubjectTypes.Pairwise];
                cfg.IdTokenSigningAlgValuesSupported = [SecurityAlgorithms.RsaSha256];
                cfg.TokenEndpointAuthMethodsSupported =
                [
                    TokenAuthMethods.ClientSecretBasic,
                    TokenAuthMethods.ClientSecretJwt,
                    TokenAuthMethods.ClientSecretPost,
                    TokenAuthMethods.PrivateKeyJwt,
                    TokenAuthMethods.None
                ];

                cfg.CodeChallengeMethodsSupported = [ChallengeMethods.S256];
                cfg.ScopesSupported =
                [
                    Scopes.OfflineAccess,
                    Scopes.OpenId,
                    Scopes.Email,
                    Scopes.Profile,
                    Scopes.Phone,
                    Scopes.Address
                ];

                cfg.BackchannelLogoutSupported = false;
                cfg.BackchannelLogoutSessionSupported = false;
            
                cfg.FrontchannelLogoutSupported = true;
                cfg.FrontchannelLogoutSessionSupported = true;
            });
        }

        private void AddClientManagement()
        {
            services.AddScoped<IClientManager, ClientManager>();
        }

        private void AddAuthorizationCodeManagement()
        {
            services.AddScoped<IAuthorizationCodeManager, AuthorizationCodeManager>();
        }

        private void AddOpenidConfiguration(Action<OpenIdOptions> configure)
        {
            services.Configure(configure);
        }

        private void AddLogoutFlow()
        {
            services.AddScoped<ILogoutStrategyResolver, LogoutStrategyResolver>();
            services.AddKeyedScoped<ILogoutStrategy, ManualLogoutStrategy>(LogoutType.Manual);
        }

        private void AddPkceHandler()
        {
            services.AddScoped<IPkceHandler, PkceHandler>();
        }

        private void AddSession(Action<SessionOptions> configure)
        {
            services.AddScoped<ISessionManager, SessionManager>();
            services.Configure(configure);
        }
        
        public void AddTokenFlow()
        {
            services.AddScoped<ITokenManager, TokenManager>();
            services.AddSingleton<IClaimBuilderFactory, ClaimBuilderFactory>();
            services.AddScoped<ITokenStrategyResolver, TokenStrategyResolver>();
            services.AddKeyedScoped<ITokenStrategy, AuthorizationCodeStrategy>(GrantTypes.AuthorizationCode);
            services.AddKeyedScoped<ITokenStrategy, RefreshTokenStrategy>(GrantTypes.RefreshToken);
        }
    }
}