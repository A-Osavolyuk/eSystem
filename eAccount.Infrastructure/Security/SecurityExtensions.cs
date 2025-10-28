using eAccount.Infrastructure.Security.Authentication.JWT;
using eAccount.Infrastructure.Security.Authentication.SSO.Clients;
using eAccount.Infrastructure.Security.Credentials.PublicKey;
using eAccount.Infrastructure.Security.Cryptography.Keys;
using eAccount.Infrastructure.Security.Cryptography.Protection;
using eSystem.Core.Security.Authentication.Cookies;
using eSystem.Core.Security.Authentication.SSO.Constants;
using eSystem.Core.Security.Cryptography.Keys;
using eSystem.Core.Security.Cryptography.Protection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace eAccount.Infrastructure.Security;

public static class SecurityExtensions
{
    public static void AddAuthentication(this IHostApplicationBuilder builder)
    {
        builder.Services.ConfigureClient(cfg =>
        {
            cfg.ClientId = "eAccount";
            cfg.ClientSecret = "2f213a036e325a55dc19320f03c2fad7c13f0169788b5968686cb4931341c393a651d7e6";
            cfg.RedirectUri = "http://localhost:5501/sso/callback";
            cfg.Scopes =
            [
                Scopes.Address,
                Scopes.Email,
                Scopes.OpenId,
                Scopes.PhoneNumber,
                Scopes.Profile,
            ];
        });
        builder.Services.AddAuthorization();
        builder.Services.AddProtection();
        builder.Services.AddScoped<IKeyFactory, RandomKeyFactory>();
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = DefaultCookies.State;
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Strict;
                
                options.ExpireTimeSpan = TimeSpan.FromDays(30);
                options.SlidingExpiration = true;
                
                options.LoginPath = "/account/login";
                options.LogoutPath = "/account/logout";
                options.AccessDeniedPath = "/access-denied";
                options.ReturnUrlParameter = "return_url";
            })
            .AddScheme<JwtAuthenticationOptions, JwtAuthenticationHandler>(
                JwtBearerDefaults.AuthenticationScheme, _ => { });

        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddScoped<TokenProvider>();
        builder.Services.AddScoped<AuthenticationManager>();
        builder.Services.AddScoped<PasskeyManager>();
        builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
    }
    
    private static void AddProtection(this IServiceCollection services)
    {
        services.AddDataProtection();
        services.AddScoped<IProtectorFactory, ProtectorFactory>();
        services.AddKeyedScoped<IProtector, SessionProtector>(ProtectionPurposes.Session);
    }
}