using eSecurity.Server.Security.Authentication.Handlers;
using eSecurity.Server.Security.Authentication.Lockout;
using eSecurity.Server.Security.Authentication.OpenIdConnect;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Constants;
using eSecurity.Server.Security.Authentication.Password;
using eSecurity.Server.Security.Authentication.Session;
using eSecurity.Server.Security.Authentication.SignIn;
using eSecurity.Server.Security.Authentication.Subject;
using eSecurity.Server.Security.Authentication.TwoFactor;
using eSecurity.Server.Security.Authorization.Constants;
using eSecurity.Server.Security.Cookies;
using eSystem.Core.Common.Configuration;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;
using eSystem.Core.Security.Authentication.Schemes;
using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Security.Authorization.OAuth.Constants;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace eSecurity.Server.Security.Authentication;

public static class AuthenticationExtensions
{
    public static void AddAuthentication(this IHostApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        builder.Services.AddScoped<IAuthenticationSessionManager, AuthenticationSessionManager>();

        builder.Services.AddSubjects(options =>
        {
            options.PublicSubjectLength = 36;
            options.PairwiseSubjectSalt = "mGz6W3q5K7YwL0rVb3nA0zWQY9uHc2pQ8FJxT4eRkUs=";
        });
        
        builder.Services.AddPasswordManagement();
        builder.Services.AddSignInStrategies();
        builder.Services.Add2Fa();
        builder.Services.AddLockout();
        builder.Services.AddOpenIdConnect(cfg =>
        {
            cfg.Issuer = "https://localhost:6201";
            cfg.AuthorizationEndpoint = "https://localhost:6501/connect/authorize";
            cfg.EndSessionEndpoint = "https://localhost:6501/connect/logout";
            cfg.TokenEndpoint = "https://localhost:6201/api/v1/connect/token";
            cfg.UserinfoEndpoint = "https://localhost:6201/api/v1/connect/userinfo";
            cfg.IntrospectionEndpoint = "https://localhost:6201/api/v1/connect/introspection";
            cfg.RevocationEndpoint = "https://localhost:6201/api/v1/connect/revocation";
            cfg.JwksUri = "https://localhost:6201/api/v1/connect/.well-known/jwks.json";
            cfg.DeviceAuthorizationEndpoint = "https://localhost:6201/api/v1/connect/device_authorization";

            cfg.ResponseTypesSupported = [ResponseTypes.Code];
            cfg.GrantTypesSupported =
            [
                GrantTypes.AuthorizationCode,
                GrantTypes.RefreshToken,
                GrantTypes.ClientCredentials,
                GrantTypes.DeviceCode,
                GrantTypes.TokenExchange
            ];

            cfg.PromptValuesSupported =
            [
                PromptTypes.None,
                PromptTypes.Login,
                PromptTypes.Consent,
                PromptTypes.SelectAccount
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
                ScopeTypes.OfflineAccess,
                ScopeTypes.OpenId,
                ScopeTypes.Email,
                ScopeTypes.Profile,
                ScopeTypes.Phone,
                ScopeTypes.Address,
                ScopeTypes.ClientRegistration,
                ScopeTypes.Transformation,
                ScopeTypes.Delegation
            ];

            cfg.BackchannelLogoutSupported = true;
            cfg.BackchannelLogoutSessionSupported = true;

            cfg.FrontchannelLogoutSupported = true;
            cfg.FrontchannelLogoutSessionSupported = true;
        });

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = ExternalAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(ExternalAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = DefaultCookies.External;
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.MaxAge = TimeSpan.FromMinutes(5);
            })
            .AddGoogle(options =>
            {
                var settings = configuration.Get<OAuthOptions>("Providers:Google");

                options.ClientId = settings.ClientId;
                options.ClientSecret = settings.ClientSecret;
                options.SaveTokens = settings.SaveTokens;
                options.CallbackPath = settings.CallbackPath;
                options.SignInScheme = ExternalAuthenticationDefaults.AuthenticationScheme;
            })
            .AddFacebook(options =>
            {
                var settings = configuration.Get<OAuthOptions>("Providers:Facebook");

                options.ClientId = settings.ClientId;
                options.ClientSecret = settings.ClientSecret;
                options.SaveTokens = settings.SaveTokens;
                options.CallbackPath = settings.CallbackPath;
                options.SignInScheme = ExternalAuthenticationDefaults.AuthenticationScheme;
            })
            .AddMicrosoftAccount(options =>
            {
                var settings = configuration.Get<OAuthOptions>("Providers:Microsoft");

                options.ClientId = settings.ClientId;
                options.ClientSecret = settings.ClientSecret;
                options.SaveTokens = settings.SaveTokens;
                options.CallbackPath = settings.CallbackPath;
                options.SignInScheme = ExternalAuthenticationDefaults.AuthenticationScheme;
            })
            .AddScheme<JwtAuthenticationSchemeOptions, JwtAuthenticationHandler>(
                JwtBearerDefaults.AuthenticationScheme, _ => { })
            .AddScheme<ClientSecretBasicAuthenticationSchemeOptions, ClientSecretBasicAuthenticationHandler>(
                ClientSecretBasicAuthenticationDefaults.AuthenticationScheme, _ => { })
            .AddScheme<ClientSecretPostAuthenticationSchemeOptions, ClientSecretPostAuthenticationHandler>(
                ClientSecretPostAuthenticationDefaults.AuthenticationScheme, _ => { });
    }
}