using eSecurity.Core.Security.Cookies.Constants;
using eSecurity.Server.Security.Authentication.Handlers;
using eSecurity.Server.Security.Authentication.Lockout;
using eSecurity.Server.Security.Authentication.Oidc;
using eSecurity.Server.Security.Authentication.Oidc.Constants;
using eSecurity.Server.Security.Authentication.Password;
using eSecurity.Server.Security.Authentication.SignIn;
using eSecurity.Server.Security.Authentication.TwoFactor;
using eSecurity.Server.Security.Authorization.OAuth;
using eSecurity.Server.Security.Authorization.OAuth.Schemes;
using eSystem.Core.Common.Configuration;
using eSystem.Core.Security.Authentication.Oidc;
using eSystem.Core.Security.Authentication.Oidc.Authorization;
using eSystem.Core.Security.Authentication.Oidc.Token;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace eSecurity.Server.Security.Authentication;

public static class AuthenticationExtensions
{
    public static void AddAuthentication(this IHostApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        
        builder.Services.AddPasswordManagement();
        builder.Services.AddSignInStrategies();
        builder.Services.Add2Fa();
        builder.Services.AddLockout();
        builder.Services.AddOidc(cfg =>
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
        
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = AuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(AuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = DefaultCookies.External;
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.MaxAge = TimeSpan.FromDays(30);
            })
            .AddGoogle(options =>
            {
                var settings =
                    configuration.Get<OAuthOptions>("Configuration:Security:Authentication:Providers:Google");

                options.ClientId = settings.ClientId;
                options.ClientSecret = settings.ClientSecret;
                options.SaveTokens = settings.SaveTokens;
                options.CallbackPath = settings.CallbackPath;
            })
            .AddFacebook(options =>
            {
                var settings =
                    configuration.Get<OAuthOptions>("Configuration:Security:Authentication:Providers:Facebook");

                options.ClientId = settings.ClientId;
                options.ClientSecret = settings.ClientSecret;
                options.SaveTokens = settings.SaveTokens;
                options.CallbackPath = settings.CallbackPath;
            })
            .AddMicrosoftAccount(options =>
            {
                var settings =
                    configuration.Get<OAuthOptions>("Configuration:Security:Authentication:Providers:Microsoft");

                options.ClientId = settings.ClientId;
                options.ClientSecret = settings.ClientSecret;
                options.SaveTokens = settings.SaveTokens;
                options.CallbackPath = settings.CallbackPath;
            })
            .AddScheme<JwtAuthenticationSchemeOptions, JwtAuthenticationHandler>(
                JwtBearerDefaults.AuthenticationScheme, _ => {})
            .AddScheme<BasicAuthenticationSchemeOptions, BasicAuthenticationHandler>(
                BasicAuthenticationDefaults.AuthenticationScheme, _ => {});
    }
}