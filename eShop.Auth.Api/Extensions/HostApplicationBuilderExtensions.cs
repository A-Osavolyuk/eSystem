using eShop.Application.Security.Authorization.Requirements;
using eShop.Auth.Api.Security.Schemes;
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
        builder.AddServices<IAssemblyMarker>();
        builder.AddRedisCache();
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
    
    private static void AddRedisCache(this IHostApplicationBuilder builder)
    {
        builder.AddRedisClient("redis");
    }

    private static void AddMediatR(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<IAssemblyMarker>();
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
        builder.AddSqlServerDbContext<AuthDbContext>("auth-db",
            configureDbContextOptions: cfg =>
            {
                cfg.UseAsyncSeeding(async (ctx, _, ct) =>
                {
                    var context = (ctx as AuthDbContext)!;
                    await context.SeedAsync(ct);
                });
            });
    }

    private static void AddIdentity(this IHostApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        builder.Services.Configure<JwtOptions>(configuration.GetSection("Jwt"));

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

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("DeleteAccountPolicy", policy => { policy.Requirements.Add(new PermissionRequirement("DELETE_ACCOUNT")); })
            .AddPolicy("CreateAccountPolicy", policy => policy.Requirements.Add(new PermissionRequirement("CREATE_ACCOUNT")))
            .AddPolicy("UpdateAccountPolicy", policy => policy.Requirements.Add(new PermissionRequirement("UPDATE_ACCOUNT")))
            .AddPolicy("ReadAccountPolicy", policy => policy.Requirements.Add(new PermissionRequirement("READ_ACCOUNT")))
            
            .AddPolicy("DeleteUserPolicy", policy => policy.Requirements.Add(new PermissionRequirement("DELETE_USER")))
            .AddPolicy("CreateUserPolicy", policy => policy.Requirements.Add(new PermissionRequirement("CREATE_USER")))
            .AddPolicy("UpdateUserPolicy", policy => policy.Requirements.Add(new PermissionRequirement("UPDATE_USER")))
            .AddPolicy("ReadUserPolicy", policy => policy.Requirements.Add(new PermissionRequirement("READ_USER")))
            .AddPolicy("LockoutUserPolicy", policy => policy.Requirements.Add(new PermissionRequirement("LOCKOUT_USER")))
            .AddPolicy("UnlockUserPolicy", policy => policy.Requirements.Add(new PermissionRequirement("UNLOCK_USER")))
            
            .AddPolicy("DeleteRolePolicy", policy => policy.Requirements.Add(new PermissionRequirement("DELETE_ROLE")))
            .AddPolicy("CreateRolePolicy", policy => policy.Requirements.Add(new PermissionRequirement("CREATE_ROLE")))
            .AddPolicy("UpdateRolePolicy", policy => policy.Requirements.Add(new PermissionRequirement("UPDATE_ROLE")))
            .AddPolicy("ReadRolePolicy", policy => policy.Requirements.Add(new PermissionRequirement("READ_ROLE")))
            .AddPolicy("AssignRolePolicy", policy => policy.Requirements.Add(new PermissionRequirement("ASSIGN_ROLE")))
            .AddPolicy("UnassignRolePolicy", policy => policy.Requirements.Add(new PermissionRequirement("UNASSIGN_ROLE")))
            
            .AddPolicy("DeletePermissionPolicy", policy => policy.Requirements.Add(new PermissionRequirement("DELETE_PERMISSION")))
            .AddPolicy("CreatePermissionPolicy", policy => policy.Requirements.Add(new PermissionRequirement("CREATE_PERMISSION")))
            .AddPolicy("UpdatePermissionPolicy", policy => policy.Requirements.Add(new PermissionRequirement("UPDATE_PERMISSION")))
            .AddPolicy("ReadPermissionPolicy",policy => policy.Requirements.Add(new PermissionRequirement("READ_PERMISSIONS")))
            .AddPolicy("GrantPermissionPolicy", policy => policy.Requirements.Add(new PermissionRequirement("GRANT_PERMISSIONS")))
            .AddPolicy("RevokePermissionPolicy", policy => policy.Requirements.Add(new PermissionRequirement("REVOKE_PERMISSIONS")));
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
}