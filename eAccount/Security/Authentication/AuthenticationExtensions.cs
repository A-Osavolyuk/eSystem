using eAccount.Security.Authentication.JWT;
using eAccount.Security.Authentication.ODIC;
using eAccount.Security.Authentication.ODIC.Clients;
using eAccount.Security.Authentication.TwoFactor;
using eSystem.Core.Security.Authentication.ODIC.Constants;
using eSystem.Core.Security.Cookies;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;

namespace eAccount.Security.Authentication;

public static class AuthenticationExtensions
{
    public static void AddAuthentication(this IHostApplicationBuilder builder)
    {
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddScoped<TokenProvider>();
        builder.Services.AddScoped<AuthenticationManager>();
        builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
        
        builder.Services.AddScoped<ITwoFactorService, TwoFactorService>();
        builder.Services.AddScoped<IConnectService, ConnectService>();
        
        builder.Services.ConfigureClient(cfg =>
        {
            cfg.ClientId = "eAccount";
            cfg.ClientSecret = "2f213a036e325a55dc19320f03c2fad7c13f0169788b5968686cb4931341c393a651d7e6";
            cfg.RedirectUri = "http://localhost:5501/connect/callback";
            cfg.Scopes =
            [
                Scopes.Address,
                Scopes.Email,
                Scopes.OpenId,
                Scopes.Phone,
                Scopes.Profile,
            ];
        });
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
    }
}