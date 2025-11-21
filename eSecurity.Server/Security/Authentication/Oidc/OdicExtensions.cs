using eSecurity.Core.Security.Authentication.Oidc;
using eSecurity.Server.Security.Authentication.Oidc.Client;
using eSecurity.Server.Security.Authentication.Oidc.Code;
using eSecurity.Server.Security.Authentication.Oidc.Constants;
using eSecurity.Server.Security.Authentication.Oidc.Pkce;
using eSecurity.Server.Security.Authentication.Oidc.Session;
using eSecurity.Server.Security.Authentication.Oidc.Token;
using eSecurity.Server.Security.Authentication.Oidc.Token.Strategies;
using eSecurity.Server.Security.Identity.Claims;
using eSystem.Core.Security.Authentication.Oidc.Authorization;
using eSystem.Core.Security.Authentication.Oidc.Constants;
using eSystem.Core.Security.Authentication.Oidc.Token;
using SessionOptions = eSecurity.Server.Security.Authentication.Oidc.Session.SessionOptions;

namespace eSecurity.Server.Security.Authentication.Oidc;

public static class OdicExtensions
{
    extension(IServiceCollection services)
    {
        public void AddOidc()
        {
            services.AddPkceHandler();
            services.AddTokenFlow();
            services.AddClientManagement();
            services.AddAuthorizationCodeManagement();
            services.AddSession(cfg => { cfg.Timestamp = TimeSpan.FromDays(30); });

            services.AddOpenidConfiguration(cfg =>
            {
                cfg.Issuer = "https://localhost:6201";
                cfg.AuthorizationEndpoint = "https://localhost:6501/connect/authorize";
                cfg.EndSessionEndpoint = "https://localhost:6501/connect/logout";
                cfg.TokenEndpoint = "https://localhost:6201/api/v1/connect/token";
                cfg.UserinfoEndpoint = "https://localhost:6201/api/v1/connect/userinfo";
                cfg.IntrospectionEndpoint = "https://localhost:6201/api/v1/connect/introspection";
                cfg.RevocationEndpoint = "https://localhost:6201/api/v1/connect/revocation";
                cfg.JwksUri = "https://localhost:6201/api/v1/connect/jwks.json";

                cfg.ResponseTypesSupported = [ResponseTypes.Code];
                cfg.GrantTypesSupported =
                [
                    GrantTypes.AuthorizationCode,
                    GrantTypes.RefreshToken
                ];
                
                cfg.PromptValuesSupported =
                [
                    Prompts.None,
                    Prompts.Login,
                    Prompts.Consent,
                    Prompts.SelectAccount
                ];

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