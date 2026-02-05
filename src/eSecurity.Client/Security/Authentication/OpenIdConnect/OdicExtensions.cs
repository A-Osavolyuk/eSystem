using eSecurity.Client.Security.Authentication.OpenIdConnect.Authorization;
using eSecurity.Client.Security.Authentication.OpenIdConnect.Prompts;
using eSecurity.Client.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Client.Security.Authentication.OpenIdConnect.Token;
using eSystem.Core.Security.Authentication.OpenIdConnect.Client;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;

namespace eSecurity.Client.Security.Authentication.OpenIdConnect;

public static class OdicExtensions
{
    extension(IServiceCollection services)
    {
        public void AddOidc()
        {
            services.AddScoped<ITokenValidator, TokenValidator>();
            services.AddScoped<ISessionAccessor, SessionAccessor>();
            services.AddScoped<IOpenIdConnectAuthorizationHandler, OpenIdConnectAuthorizationHandler>();

            services.AddScoped<IPromptStrategy, LoginPromptStrategy>();
            services.AddScoped<IPromptStrategy, ConsentPromptStrategy>();
            services.AddScoped<IPromptStrategy, SelectAccountPromptStrategy>();
            services.AddScoped<IPromptStrategy, NonePromptStrategy>();

            services.AddClient(cfg =>
            {
                cfg.ClientAudience = "api://esecurity";
                cfg.ClientId = "392e390f-33bd-4f30-af70-ccbe04bbb2c4";
                cfg.ClientSecret = "09ba08a500f11aa321fb1dfd8c40ed017e2c1b70f992b86dac6d591089e33cb2";
                cfg.CallbackUri = "https://localhost:6501/connect/callback";
                cfg.PostLogoutRedirectUri = "https://localhost:6501/login";
                cfg.SupportedScopes =
                [
                    ScopeTypes.OpenId,
                    ScopeTypes.OfflineAccess,
                    ScopeTypes.Email,
                    ScopeTypes.Phone,
                    ScopeTypes.Profile,
                    ScopeTypes.Address
                ];
                cfg.SupportedPrompts =
                [
                    PromptTypes.Login,
                    PromptTypes.Consent,
                    PromptTypes.SelectAccount,
                    PromptTypes.None
                ];
            });
        }
    }
}