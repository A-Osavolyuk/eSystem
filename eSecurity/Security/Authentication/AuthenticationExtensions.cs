using eSecurity.Security.Authentication.Schemes;
using eSecurity.Security.Authorization.OAuth;
using eSecurity.Security.Authentication.JWT;
using eSecurity.Security.Authentication.Lockout;
using eSecurity.Security.Authentication.ODIC;
using eSecurity.Security.Authentication.Password;
using eSecurity.Security.Authentication.SignIn;
using eSecurity.Security.Authentication.TwoFactor;
using eSystem.Core.Common.Configuration;
using eSystem.Core.Security.Authentication.JWT;
using eSystem.Core.Security.Cookies;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace eSecurity.Security.Authentication;

public static class AuthenticationExtensions
{
    public static void AddAuthentication(this IHostApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        
        builder.Services.AddPasswordManagement();
        builder.Services.AddSignInStrategies();
        builder.Services.Add2FA();
        builder.Services.AddJwt();
        builder.Services.AddLockout();
        builder.Services.AddOdic();

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = AuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(AuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = DefaultCookies.External;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            })
            .AddGoogle(options =>
            {
                var settings =
                    configuration.Get<ProviderOptions>("Configuration:Security:Authentication:Providers:Google");

                options.ClientId = settings.ClientId;
                options.ClientSecret = settings.ClientSecret;
                options.SaveTokens = settings.SaveTokens;
                options.CallbackPath = settings.CallbackPath;
            })
            .AddFacebook(options =>
            {
                var settings =
                    configuration.Get<ProviderOptions>("Configuration:Security:Authentication:Providers:Facebook");

                options.ClientId = settings.ClientId;
                options.ClientSecret = settings.ClientSecret;
                options.SaveTokens = settings.SaveTokens;
                options.CallbackPath = settings.CallbackPath;
            })
            .AddMicrosoftAccount(options =>
            {
                var settings =
                    configuration.Get<ProviderOptions>("Configuration:Security:Authentication:Providers:Microsoft");

                options.ClientId = settings.ClientId;
                options.ClientSecret = settings.ClientSecret;
                options.SaveTokens = settings.SaveTokens;
                options.CallbackPath = settings.CallbackPath;
            })
            .AddJwtBearer(options =>
            {
                var settings = configuration.Get<JwtOptions>("Jwt");
                var encodedKey = Encoding.UTF8.GetBytes(settings.Secret);
                var symmetricSecurityKey = new SymmetricSecurityKey(encodedKey);

                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = settings.Audience,
                    ValidIssuer = settings.Issuer,
                    IssuerSigningKey = symmetricSecurityKey
                };
            });
    }
}