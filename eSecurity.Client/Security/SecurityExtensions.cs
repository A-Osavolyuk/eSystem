using eSecurity.Client.Security.Authentication;
using eSecurity.Client.Security.Authentication.Oidc;
using eSecurity.Client.Security.Authentication.Oidc.Token;
using eSecurity.Client.Security.Authentication.TwoFactor;
using eSecurity.Client.Security.Authorization.Access.Verification;
using eSecurity.Client.Security.Authorization.Devices;
using eSecurity.Client.Security.Authorization.LinkedAccounts;
using eSecurity.Client.Security.Authorization.OAuth;
using eSecurity.Client.Security.Credentials.PublicKey;
using eSecurity.Client.Security.Identity;
using eSecurity.Core.Security.Cookies;
using eSecurity.Core.Security.Cookies.Constants;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;

namespace eSecurity.Client.Security;

public static class SecurityExtensions
{
    extension(IServiceCollection services)
    {
        public void AddSecurity()
        {
            services.AddAuthorization();
            services.AddOidc();
            services.AddCookies();

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
            });
            
            services.AddScoped<TokenProvider>();
            services.AddScoped<AuthenticationManager>();
            services.AddScoped<AuthenticationStateProvider, ClaimAuthenticationStateProvider>();
            
            services.AddScoped<IOAuthService, OAuthService>();
            services.AddScoped<ILinkedAccountService, LinkedAccountService>();
            services.AddScoped<IDeviceService, DeviceService>();
            services.AddScoped<IVerificationService, VerificationService>();
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<IConnectService, ConnectService>();
            services.AddScoped<ITwoFactorService, TwoFactorService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPasskeyService, PasskeyService>();
        }
    }
}