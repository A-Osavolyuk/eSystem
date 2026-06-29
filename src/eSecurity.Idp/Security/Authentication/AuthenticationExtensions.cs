using eSecurity.Idp.Security.Authentication.AuthenticationSession;
using eSecurity.Idp.Security.Authentication.BackchannelAuthentication;
using eSecurity.Idp.Security.Authentication.Ciba;
using eSecurity.Idp.Security.Authentication.Client;
using eSecurity.Idp.Security.Authentication.EndSession;
using eSecurity.Idp.Security.Authentication.Handlers;
using eSecurity.Idp.Security.Cookies;
using eSecurity.Idp.Security.Authentication.Lockout;
using eSecurity.Idp.Security.Authentication.OpenIdConnect;
using eSecurity.Idp.Security.Authentication.Password.Extensions;
using eSecurity.Idp.Security.Authentication.Session.Extensions;
using eSecurity.Idp.Security.Authentication.SignIn.Extensions;
using eSecurity.Idp.Security.Authentication.Subject;
using eSecurity.Idp.Security.Authentication.Subject.Extensions;
using eSecurity.Idp.Security.Authentication.TwoFactor.Extensions;
using eSecurity.Idp.Security.Cryptography.Pkce;
using eSystem.Core.Configuration;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Security.Authentication.OpenIdConnect.Client;
using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Server.Security.Authentication.Schemes;
using eSystem.Core.Server.Security.Authorization.OAuth;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace eSecurity.Idp.Security.Authentication;

public static class AuthenticationExtensions
{
    public static void AddAuthentication(this IHostApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var clientUri = configuration.GetValue<string>("Client:Uri");

        builder.Services.AddScoped<IAuthenticationSessionManager, AuthenticationSessionManager>();
        
        builder.Services.AddScoped<IPkceHandler, PkceHandler>();
        builder.Services.AddScoped<IClientQueryService, ClientQueryService>();
        builder.Services.AddScoped<IClientCommandService, ClientCommandService>();
        builder.Services.AddScoped<ICibaRequestManager, CibaRequestManager>();
            
        builder.Services.AddBackchannelAuthentication(options =>
        {
            options.Interval = 5;
            options.AuthReqIdLength = 32;
            options.DefaultRequestLifetime = TimeSpan.FromSeconds(300);
            options.MinRequestLifetime = TimeSpan.FromSeconds(60);
            options.MaxRequestLifetime = TimeSpan.FromSeconds(600);
        });
            
        builder.Services.AddSsoSessions(cfg =>
        {
            cfg.Timestamp = TimeSpan.FromDays(30);
        });
        
        builder.Services.AddSubjects(options =>
        {
            options.PublicSubjectLength = 36;
            options.PairwiseSubjectSalt = "mGz6W3q5K7YwL0rVb3nA0zWQY9uHc2pQ8FJxT4eRkUs=";
        });
        
        builder.Services.AddPasswordManagement();
        builder.Services.AddSignInStrategies();
        builder.Services.Add2Fa();
        builder.Services.AddLockout();
        
        builder.Services.AddEndSessionFlow(options =>
        {
            options.LogoutUrl = $"{clientUri}/connect/logout";
            options.LoggedOutUrl = $"{clientUri}/connect/logged-out";
            options.FallbackUrl = $"{clientUri}/connect/logout/fallback";
            options.Timestamp = TimeSpan.FromMinutes(5);
            options.FrontchannelRedirectDelay = TimeSpan.FromMilliseconds(500);
        });
        
        builder.Services.AddOpenIdConnect(cfg =>
        {
            cfg.Issuer = "https://localhost:6201";
            cfg.AuthorizationEndpoint = "https://localhost:6201/api/v1/connect/authorize";
            cfg.EndSessionEndpoint = "https://localhost:6201/api/v1/connect/end-session";
            cfg.TokenEndpoint = "https://localhost:6201/api/v1/connect/token";
            cfg.UserinfoEndpoint = "https://localhost:6201/api/v1/connect/userinfo";
            cfg.IntrospectionEndpoint = "https://localhost:6201/api/v1/connect/introspection";
            cfg.RevocationEndpoint = "https://localhost:6201/api/v1/connect/revocation";
            cfg.JwksUri = "https://localhost:6201/api/v1/connect/.well-known/jwks.json";
            cfg.DeviceAuthorizationEndpoint = "https://localhost:6201/api/v1/connect/device-authorization";
            cfg.BackchannelAuthenticationEndpoint = "https://localhost:6201/api/v1/connect/backchannel-authentication";

            // Pushed Authorization Request
            cfg.PushedAuthorizationRequestEndpoint = "https://localhost:6201/api/v1/connect/par";
            cfg.RequestUriParameterSupported = true;
            cfg.RequireRequestUriRegistration = false;

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