using eSecurity.Client.Security.Authentication;
using eSecurity.Client.Security.Authentication.Handlers.Jwt;
using eSecurity.Client.Security.Authentication.Odic;
using eSecurity.Client.Security.Authentication.Odic.Token;
using eSecurity.Core.Security.Cookies;
using eSecurity.Core.Security.Cookies.Constants;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;

namespace eSecurity.Client.Security;

public static class SecurityExtensions
{
    extension(IServiceCollection services)
    {
        public void AddSecurity()
        {
            services.AddOdic();
            services.AddCookies();
            services.AddScoped<TokenProvider>();
            services.AddScoped<AuthenticationManager>();
            services.AddScoped<AuthenticationStateProvider, ClaimAuthenticationStateProvider>();
        
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Cookie.Name = DefaultCookies.State;
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SameSite = SameSiteMode.Strict;

                    options.ExpireTimeSpan = TimeSpan.FromDays(30);
                    options.SlidingExpiration = true;

                    options.LoginPath = "/account/sign-in";
                    options.LogoutPath = "/account/logout";
                    options.AccessDeniedPath = "/access-denied";
                    options.ReturnUrlParameter = "return_url";
                })
                .AddScheme<JwtAuthenticationOptions, JwtAuthenticationHandler>(
                    JwtBearerDefaults.AuthenticationScheme, _ => {});
        }
    }
}