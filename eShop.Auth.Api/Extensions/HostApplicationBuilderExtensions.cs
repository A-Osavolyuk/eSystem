
using eShop.Auth.Api.Security.Credentials.PublicKey;
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
        builder.Services.AddCryptography();
        builder.Services.AddJwt();
        builder.Services.Add2FA();
        builder.Services.AddCredentials(cfg =>
        {
            var options = configuration.Get<CredentialOptions>("Configuration:Security:Credentials");
            
            cfg.Domain = options.Domain;
            cfg.Server = options.Server;
            cfg.Timeout = options.Timeout;
        });
        
        builder.Services.AddIdentity(cfg =>
        {
            cfg.ConfigurePassword(options =>
            {
                options.RequiredLength = 8;
                options.RequireUppercase = true;
                options.RequiredUppercase = 1;
                options.RequireLowercase = true;
                options.RequiredLowercase = 1;
                options.RequireDigit = true;
                options.RequiredDigits = 1;
                options.RequireNonAlphanumeric = true;
                options.RequiredNonAlphanumeric = 1;
                options.RequireUniqueChars = false;
            });
            
            cfg.ConfigureAccount(options =>
            {
                options.RequireUniqueEmail = true;
                options.RequireUniqueRecoveryEmail = true;
                options.RequireUniquePhoneNumber = true;
                options.RequireUniqueUserName = true;
            
                options.PrimaryEmailMaxCount = 1;
                options.SecondaryEmailMaxCount = 3;
                options.RecoveryEmailMaxCount = 1;

                options.PrimaryPhoneNumberMaxCount = 1;
                options.SecondaryPhoneNumberMaxCount = 3;
                options.RecoveryPhoneNumberMaxCount = 1;
            });
            
            cfg.ConfigureSignIn(options =>
            {
                options.AllowUserNameLogin = true;
                options.AllowEmailLogin = true;
                options.AllowOAuthLogin = true;
                options.RequireConfirmedAccount = true;
                options.RequireConfirmedEmail = true;
                options.RequireConfirmedPhoneNumber = true;
                options.RequireConfirmedRecoveryEmail = true;
                options.RequireTrustedDevice = true;
                options.MaxFailedLoginAttempts = 5;
            });
            
            cfg.ConfigureCode(options =>
            {
                options.MaxCodeResendAttempts = 5;
                options.CodeResendUnavailableTime = 2;
            });
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