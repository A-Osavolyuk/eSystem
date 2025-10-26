using eAccount.Infrastructure.Security.Cryptography.Protection;
using eSystem.Core.Security.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace eAccount.Infrastructure.Security;

public static class SecurityExtensions
{
    public static void AddAuthentication(this IHostApplicationBuilder builder)
    {
        builder.Services.AddAuthorization();
        builder.Services.AddProtection();
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
}