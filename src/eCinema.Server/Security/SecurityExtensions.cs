using eCinema.Server.Security.Authentication.Oidc;
using eCinema.Server.Security.Authentication.Oidc.Session;
using eCinema.Server.Security.Authentication.Oidc.Token;
using eSystem.Core.Security.Authentication.Oidc.Client;
using eSystem.Core.Security.Authentication.Oidc.Constants;

namespace eCinema.Server.Security;

public static class SecurityExtensions
{
    public static void AddSecurity(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IOpenIdDiscoveryProvider, OpenIdDiscoveryProvider>();
        builder.Services.AddScoped<ITokenValidator, TokenValidator>();
        builder.Services.AddScoped<ITokenProvider, TokenProvider>();
        builder.Services.AddScoped<IConnectService, ConnectService>();
        builder.Services.AddScoped<ISessionProvider, SessionProvider>();
        
        builder.Services.AddClient(cfg =>
        {
            cfg.ClientAudience = "eCinema";
            cfg.ClientId = "307268b0-005c-4ee4-a0e8-a93bd0010382";
            cfg.ClientSecret = "7fd5a079ecd90974a56532138e204ec0c42df875a06a0dedbe69797b609150c10162abed";
            cfg.CallbackUri = "https://localhost:6204/api/v1/connect/callback";
            cfg.PostLogoutRedirectUri = "https://localhost:6502/connect/logged-out";
            cfg.SupportedScopes =
            [
                Scopes.OpenId,
                Scopes.OfflineAccess,
                Scopes.Email,
                Scopes.Phone,
                Scopes.Profile,
                Scopes.Address
            ];
            cfg.SupportedPrompts =
            [
                Prompts.Login,
                Prompts.Consent
            ];
        });
    }
}