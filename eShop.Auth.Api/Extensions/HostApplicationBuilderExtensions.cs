
using eShop.Auth.Api.Security.Jwt;
using eShop.Auth.Api.Security.Schemes;
using eShop.Auth.Api.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace eShop.Auth.Api.Extensions;

public static class HostApplicationBuilderExtensions
{
    public static void AddApiServices(this IHostApplicationBuilder builder)
    {
        builder.AddVersioning();
        builder.AddMessageBus();
        builder.AddValidation<IAssemblyMarker>();
        builder.AddServiceDefaults();
        builder.AddSecurity();
        builder.AddServices<IAssemblyMarker>();
        builder.AddRedisCache();
        builder.AddMediatR();
        builder.AddMsSqlDb();
        builder.AddLogging();
        builder.AddExceptionHandler();
        builder.AddDocumentation();
        builder.AddVerification();
        builder.AddSession();
        
        builder.Services.AddControllers()
            .AddJsonOptions(cfg => cfg.JsonSerializerOptions.WriteIndented = true);
        
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddDistributedMemoryCache();
    }

    private static void AddSession(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<ISessionStorage, SessionStorage>();
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(5);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });
    }

    private static void AddMediatR(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMediatR(cfg => { cfg.RegisterServicesFromAssemblyContaining<IAssemblyMarker>(); });
    }

    private static void AddMsSqlDb(this IHostApplicationBuilder builder)
    {
        builder.AddSqlServerDbContext<AuthDbContext>("auth-db",
            configureDbContextOptions: cfg =>
            {
                cfg.UseAsyncSeeding(async (ctx, _, ct) =>
                {
                    await ctx.SeedAsync<IAssemblyMarker>(ct);
                });
            });
    }

    private static void AddSecurity(this IHostApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        builder.Services.AddAuthorization();
        builder.Services.AddProtection();
        builder.Services.AddHashing();
        builder.Services.AddSignInStrategies();
        builder.Services.AddCredentials();
        builder.Services.AddCryptography();
        builder.Services.AddJwt();
        
        builder.Services.Add2FA(cfg =>
        {
            cfg.AddMethod(TwoFactorMethod.Sms);
            cfg.AddMethod(TwoFactorMethod.AuthenticatorApp);
            cfg.AddMethod(TwoFactorMethod.Passkey);
        });

        builder.Services.AddIdentity(options =>
        {
            options.Credentials.Domain = "localhost";
            options.Credentials.Server = "eAccount";
            options.Credentials.Timeout = 60000;
            
            options.Password.RequiredLength = 8;
            options.Password.RequireUppercase = true;
            options.Password.RequiredUppercase = 1;
            options.Password.RequireLowercase = true;
            options.Password.RequiredLowercase = 1;
            options.Password.RequireDigit = true;
            options.Password.RequiredDigits = 1;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredNonAlphanumeric = 1;
            options.Password.RequireUniqueChars = false;

            options.Account.RequireUniqueEmail = true;
            options.Account.RequireUniqueRecoveryEmail = true;
            options.Account.RequireUniquePhoneNumber = true;
            options.Account.RequireUniqueUserName = true;
            
            options.Account.PrimaryEmailMaxCount = 1;
            options.Account.SecondaryEmailMaxCount = 3;
            options.Account.RecoveryEmailMaxCount = 1;

            options.Account.PrimaryPhoneNumberMaxCount = 1;
            options.Account.SecondaryPhoneNumberMaxCount = 3;
            options.Account.RecoveryPhoneNumberMaxCount = 1;

            options.SignIn.AllowUserNameLogin = true;
            options.SignIn.AllowEmailLogin = true;
            options.SignIn.AllowOAuthLogin = true;
            options.SignIn.RequireConfirmedAccount = true;
            options.SignIn.RequireConfirmedEmail = true;
            options.SignIn.RequireConfirmedPhoneNumber = true;
            options.SignIn.RequireConfirmedRecoveryEmail = true;
            options.SignIn.RequireTrustedDevice = true;
            options.SignIn.MaxFailedLoginAttempts = 5;

            options.Code.MaxCodeResendAttempts = 5;
            options.Code.CodeResendUnavailableTime = 2;
        });

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

    private static void AddMessageBus(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                var connectionString = builder.Configuration.GetConnectionString("rabbit-mq");
                cfg.Host(connectionString);
            });
        });
    }

    private static void AddVerification(this IHostApplicationBuilder builder)
    {
        builder.Services.AddVerification(cfg =>
        {
            cfg.AddMethod(VerificationMethod.Email);
            cfg.AddMethod(VerificationMethod.Passkey);
            cfg.AddMethod(VerificationMethod.AuthenticatorApp);
        });
    }
}