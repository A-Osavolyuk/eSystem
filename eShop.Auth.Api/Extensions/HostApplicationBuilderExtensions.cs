using eShop.Auth.Api.Security.Schemes;
using eShop.Domain.Requests.API.Cart;
using eShop.Domain.Requests.API.Sms;
using MassTransit;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace eShop.Auth.Api.Extensions;

public static class HostApplicationBuilderExtensions
{
    public static void AddApiServices(this IHostApplicationBuilder builder)
    {
        builder.AddVersioning();
        builder.AddMessageBus();
        builder.AddValidation();
        builder.AddServiceDefaults();
        builder.AddIdentity();
        builder.AddDependencyInjection();
        builder.AddRedisCache();
        builder.AddCors();
        builder.AddMediatR();
        builder.AddMsSqlDb();
        builder.AddGrpc();
        builder.AddLogging();
        builder.AddExceptionHandler();
        builder.AddDocumentation();
        builder.Services.AddControllers();
    }

    private static void AddValidation(this IHostApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining<IAssemblyMarker>();
    }

    private static void AddMediatR(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMediatR(x =>
        {
            x.RegisterServicesFromAssemblyContaining<IAssemblyMarker>();
        });
    }

    private static void AddGrpc(this IHostApplicationBuilder builder)
    {
        builder.Services.AddGrpc(options =>
        {
            options.EnableDetailedErrors = true; 
        });
    }

    private static void AddMsSqlDb(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString(SqlDb.SqlServer);
        builder.Services.AddDbContext<AuthDbContext>(cfg =>
        {
            cfg.UseSqlServer(connectionString);
            cfg.UseAsyncSeeding(async (ctx, isStoreOperation, ct) =>
            {
                var context = (ctx as AuthDbContext)!;
                await context.SeedAsync(ctx, isStoreOperation, ct);
            });
        });
    }

    private static void AddIdentity(this IHostApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        
        builder.Services.Configure<JwtOptions>(configuration.GetSection("Configuration:Security:Authentication:JWT"));

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
                options.Cookie.Name = "Authentication.External";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            })
            .AddGoogle(options =>
            {
                var settings = configuration.Get<ProviderOptions>("Configuration:Security:Authentication:Providers:Google");
                
                options.ClientId = settings.ClientId ?? "";
                options.ClientSecret = settings.ClientSecret ?? "";
                options.SaveTokens = settings.SaveTokens;
                options.CallbackPath = settings.CallbackPath;
            })
            .AddFacebook(options =>
            {
                var settings = configuration.Get<ProviderOptions>("Configuration:Security:Authentication:Providers:Facebook");
                
                options.ClientId = settings.ClientId ?? "";
                options.ClientSecret = settings.ClientSecret ?? "";
                options.SaveTokens = settings.SaveTokens;
                options.CallbackPath = settings.CallbackPath;
            })
            .AddMicrosoftAccount(options =>
            {
                var settings = configuration.Get<ProviderOptions>("Configuration:Security:Authentication:Providers:Microsoft");
                
                options.ClientId = settings.ClientId ?? "";
                options.ClientSecret = settings.ClientSecret ?? "";
                options.SaveTokens = settings.SaveTokens;
                options.CallbackPath = settings.CallbackPath;
            })
            .AddJwtBearer(options =>
            {
                var settings = configuration.Get<JwtOptions>("Configuration:Security:Authentication:JWT");
                var encodedKey = Encoding.UTF8.GetBytes(settings.Key);
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

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("ManageUsersPolicy", policy => policy.Requirements.Add(new PermissionRequirement("admin:all")))
            .AddPolicy("ManageLockoutPolicy", policy => policy.Requirements.Add(new PermissionRequirement("admin:all")))
            .AddPolicy("ManageRolesPolicy", policy => policy.Requirements.Add(new PermissionRequirement("admin:all")))
            .AddPolicy("ManagePermissionsPolicy", policy => policy.Requirements.Add(new PermissionRequirement("admin:all")))
            .AddPolicy("ManageAccountPolicy", policy => policy.Requirements.Add(new PermissionRequirement("admin:all")));
    }

    private static void AddDependencyInjection(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IPermissionManager, PermissionManager>();
        builder.Services.AddScoped<ISecurityManager, SecurityManager>();
        builder.Services.AddScoped<IProfileManager, ProfileManager>();
        builder.Services.AddScoped<ICacheService, CacheService>();
        builder.Services.AddScoped<IMessageService, MessageService>();
        builder.Services.AddScoped<ISecurityTokenManager, SecurityTokenManager>();
        builder.Services.AddScoped<ICodeManager, CodeManager>();
        builder.Services.AddScoped<IRoleManager, RoleManager>();
        builder.Services.AddScoped<IUserManager, UserManager>();
        builder.Services.AddScoped<ILockoutManager, LockoutManager>();
        builder.Services.AddScoped<ITwoFactorManager, TwoFactorManager>();
        builder.Services.AddScoped<ISignInManager, SignInManager>();
        builder.Services.AddScoped<ISecretManager, SecretManager>();
        builder.Services.AddScoped<ILoginTokenManager, LoginTokenManager>();
        builder.Services.AddScoped<IProviderManager, ProviderManager>();

        builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

        builder.Services.AddHostedService<TokenValidator>();
        builder.Services.AddHostedService<CodeValidator>();
        
        builder.Services.AddScoped<CartClient>();
    }

    private static void AddMessageBus(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMassTransit(x =>
        {
            x.AddRequestClient<SingleMessageRequest>();
            x.AddRequestClient<CreateCartRequest>();
            x.UsingRabbitMq((context, cfg) =>
            {
                var uri = builder.Configuration["Configuration:Services:MessageBus:RabbitMq:HostUri"]!;
                var username = builder.Configuration["Configuration:Services:MessageBus:RabbitMq:UserName"]!;
                var password = builder.Configuration["Configuration:Services:MessageBus:RabbitMq:Password"]!;

                cfg.Host(new Uri(uri), h =>
                {
                    h.Username(username);
                    h.Password(password);
                });
            });
        });
    }
}