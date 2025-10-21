using eShop.Auth.Api.Security.Authentication.Schemes;
using eShop.Auth.Api.Security.Authentication.SignIn;
using eShop.Auth.Api.Security.Authentication.SignIn.Strategies;
using eShop.Auth.Api.Security.Authentication.TwoFactor.Authenticator;
using eShop.Auth.Api.Security.Authentication.TwoFactor.Recovery;
using eShop.Auth.Api.Security.Authorization.OAuth;
using eShop.Auth.Api.Security.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace eShop.Auth.Api.Security.Authentication;

public static class AuthenticationExtensions
{
    public static void AddAuthentication(this IHostApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        
        builder.Services.AddSignInStrategies();
        builder.Services.Add2FA();

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = ExternalAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(ExternalAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = "eAccount.Authentication.External";
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

    private static void Add2FA(this IServiceCollection services)
    {
        services.AddScoped<IQrCodeFactory, QrCodeFactory>();
        services.AddScoped<IRecoveryCodeFactory, RecoveryCodeFactory>();
    }

    private static void AddSignInStrategies(this IServiceCollection services)
    {
        services.AddScoped<ISignInResolver, SignInResolver>();
        services.AddKeyedScoped<SignInStrategy, PasswordSignInStrategy>(SignInType.Password);
        services.AddKeyedScoped<SignInStrategy, PasskeySignInStrategy>(SignInType.Passkey);
        services.AddKeyedScoped<SignInStrategy, AuthenticatorSignInStrategy>(SignInType.AuthenticatorApp);
        services.AddKeyedScoped<SignInStrategy, LinkedAccountSignInStrategy>(SignInType.LinkedAccount);
    }
}