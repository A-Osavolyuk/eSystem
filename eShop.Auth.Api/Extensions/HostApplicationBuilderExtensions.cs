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
            .AddPolicy("DeleteAccountPolicy", policy => { policy.Requirements.Add(new PermissionRequirement("Account:Delete")); })
            .AddPolicy("CreateAccountPolicy", policy => policy.Requirements.Add(new PermissionRequirement("Account:Create")))
            .AddPolicy("UpdateAccountPolicy", policy => policy.Requirements.Add(new PermissionRequirement("Account:Update")))
            .AddPolicy("ReadAccountPolicy", policy => policy.Requirements.Add(new PermissionRequirement("Account:Read")))
            
            .AddPolicy("DeleteUsersPolicy", policy => policy.Requirements.Add(new PermissionRequirement("User:Delete")))
            .AddPolicy("CreateUsersPolicy", policy => policy.Requirements.Add(new PermissionRequirement("User:Create")))
            .AddPolicy("UpdateUsersPolicy", policy => policy.Requirements.Add(new PermissionRequirement("User:Update")))
            .AddPolicy("ReadUsersPolicy", policy => policy.Requirements.Add(new PermissionRequirement("User:Read")))
            .AddPolicy("LockoutUsersPolicy", policy => policy.Requirements.Add(new PermissionRequirement("User:Lockout")))
            .AddPolicy("UnlockUsersPolicy", policy => policy.Requirements.Add(new PermissionRequirement("User:Unlock")))
            
            .AddPolicy("DeleteRolesPolicy", policy => policy.Requirements.Add(new PermissionRequirement("Role:Delete")))
            .AddPolicy("CreateRolesPolicy", policy => policy.Requirements.Add(new PermissionRequirement("Role:Create")))
            .AddPolicy("UpdateRolesPolicy", policy => policy.Requirements.Add(new PermissionRequirement("Role:Update")))
            .AddPolicy("ReadRolesPolicy", policy => policy.Requirements.Add(new PermissionRequirement("Role:Read")))
            .AddPolicy("AssignRolePolicy", policy => policy.Requirements.Add(new PermissionRequirement("Role:Assign")))
            .AddPolicy("UnassignRolePolicy", policy => policy.Requirements.Add(new PermissionRequirement("Role:Unassign")))
            
            .AddPolicy("DeletePermissionsPolicy", policy => policy.Requirements.Add(new PermissionRequirement("Permission:Delete")))
            .AddPolicy("CreatePermissionsPolicy", policy => policy.Requirements.Add(new PermissionRequirement("Permission:Create")))
            .AddPolicy("UpdatePermissionsPolicy", policy => policy.Requirements.Add(new PermissionRequirement("Permission:Update")))
            .AddPolicy("ReadPermissionsPolicy",policy => policy.Requirements.Add(new PermissionRequirement("Permission:Read")))
            .AddPolicy("GrantPermissionPolicy", policy => policy.Requirements.Add(new PermissionRequirement("Permission:Grant")))
            .AddPolicy("RevokePermissionPolicy", policy => policy.Requirements.Add(new PermissionRequirement("Permission:Revoke")));
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