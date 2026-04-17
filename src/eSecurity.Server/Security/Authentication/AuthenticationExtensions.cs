using eSecurity.Server.Security.Authentication.Handlers;
using eSecurity.Server.Security.Authentication.Lockout;
using eSecurity.Server.Security.Authentication.OpenIdConnect;
using eSecurity.Server.Security.Authentication.Password;
using eSecurity.Server.Security.Authentication.Session;
using eSecurity.Server.Security.Authentication.SignIn;
using eSecurity.Server.Security.Authentication.Subject;
using eSecurity.Server.Security.Authentication.TwoFactor;
using eSecurity.Server.Security.Cookies;
using eSystem.Core.Common.Configuration;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Security.Authentication.OpenIdConnect.Client;
using eSystem.Core.Security.Authentication.Schemes;
using eSystem.Core.Security.Authorization.OAuth;
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
            cfg.DeviceAuthorizationEndpoint = "https://localhost:6201/api/v1/connect/device-authorization";
            cfg.BackchannelAuthenticationEndpoint = "https://localhost:6201/api/v1/connect/backchannel-authentication";

            cfg.ResponseTypesSupported = [ResponseType.Code];
            cfg.GrantTypesSupported =
            [
                GrantType.AuthorizationCode,
                GrantType.RefreshToken,
                GrantType.ClientCredentials,
                GrantType.DeviceCode,
                GrantType.TokenExchange
            ];

            cfg.PromptValuesSupported =
            [
                PromptType.None,
                PromptType.Login,
                PromptType.Consent,
                PromptType.SelectAccount
            ];

            cfg.SubjectTypesSupported = [SubjectType.Public, SubjectType.Pairwise];
            cfg.IdTokenSigningAlgValuesSupported = [SecurityAlgorithms.RsaSha256];
            cfg.TokenEndpointAuthMethodsSupported =
            [
                TokenAuthMethod.ClientSecretBasic,
                TokenAuthMethod.ClientSecretJwt,
                TokenAuthMethod.ClientSecretPost,
                TokenAuthMethod.PrivateKeyJwt,
                TokenAuthMethod.None
            ];

            cfg.CodeChallengeMethodsSupported = [ChallengeMethod.S256];
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
                options.DefaultChallengeScheme = AuthenticationSchemes.External;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(AuthenticationSchemes.External, options =>
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
                options.SignInScheme = AuthenticationSchemes.External;
            })
            .AddFacebook(options =>
            {
                var settings = configuration.Get<OAuthOptions>("Providers:Facebook");

                options.ClientId = settings.ClientId;
                options.ClientSecret = settings.ClientSecret;
                options.SaveTokens = settings.SaveTokens;
                options.CallbackPath = settings.CallbackPath;
                options.SignInScheme = AuthenticationSchemes.External;
            })
            .AddMicrosoftAccount(options =>
            {
                var settings = configuration.Get<OAuthOptions>("Providers:Microsoft");

                options.ClientId = settings.ClientId;
                options.ClientSecret = settings.ClientSecret;
                options.SaveTokens = settings.SaveTokens;
                options.CallbackPath = settings.CallbackPath;
                options.SignInScheme = AuthenticationSchemes.External;
            })
            .AddScheme<JwtAuthenticationSchemeOptions, JwtAuthenticationHandler>(
                JwtBearerDefaults.AuthenticationScheme, _ => { })
            .AddScheme<ClientSecretBasicAuthenticationSchemeOptions, ClientSecretBasicAuthenticationHandler>(
                AuthenticationSchemes.ClientSecretBasic, _ => { })
            .AddScheme<ClientSecretPostAuthenticationSchemeOptions, ClientSecretPostAuthenticationHandler>(
                AuthenticationSchemes.ClientSecretPost, _ => { });
    }
}