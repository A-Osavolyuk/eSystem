namespace eShop.Auth.Api.Extensions;

public static class BuilderExtensions
{
    public static void AddApiServices(this IHostApplicationBuilder builder)
    {
        builder.Logging.AddConfiguration(builder.Configuration.GetSection("Configuration:Logging"));

        builder.AddVersioning();
        builder.AddValidation();
        builder.AddMessageBus();
        builder.AddServiceDefaults();
        builder.AddIdentity();
        builder.AddDependencyInjection();
        builder.AddRedisCache();
        builder.AddCors();
        builder.AddMediatR();
        builder.AddSqlDb();
        builder.AddGrpc();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();
        builder.Services.AddOpenApi();
    }

    private static void AddMediatR(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMediatR(x =>
        {
            x.RegisterServicesFromAssemblyContaining<IAssemblyMarker>();
            x.AddOpenBehavior(typeof(LoggingBehaviour<,>));
        });
    }

    private static void AddGrpc(this IHostApplicationBuilder builder)
    {
        builder.Services.AddGrpc(options =>
        {
            options.EnableDetailedErrors = true; 
        });
    }

    private static void AddSqlDb(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AuthDbContext>(cfg =>
        {
            cfg.UseSqlServer(builder.Configuration["Configuration:Storage:Databases:SQL:MSSQL:ConnectionString"]!);
        });
    }

    private static void AddIdentity(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<JwtOptions>(
            builder.Configuration.GetSection("Configuration:Security:Authentication:JWT"));
        
        builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = true;
        }).AddDefaultTokenProviders().AddEntityFrameworkStores<AuthDbContext>();

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddGoogle(options =>
            {
                options.ClientId =
                    builder.Configuration["Configuration:Security:Authentication:Providers:Google:ClientId"] ?? "";
                options.ClientSecret =
                    builder.Configuration["Configuration:Security:Authentication:Providers:Google:ClientSecret"] ?? "";
                options.SaveTokens = true;
                options.CallbackPath = "/signin-google";
            })
            .AddFacebook(options =>
            {
                options.ClientId =
                    builder.Configuration["Configuration:Security:Authentication:Providers:Facebook:ClientId"] ?? "";
                options.ClientSecret =
                    builder.Configuration["Configuration:Security:Authentication:Providers:Facebook:ClientSecret"] ??
                    "";
                options.SaveTokens = true;
                options.CallbackPath = "/signin-facebook";
            })
            .AddMicrosoftAccount(options =>
            {
                options.ClientId =
                    builder.Configuration["Configuration:Security:Authentication:Providers:Microsoft:ClientId"] ?? "";
                options.ClientSecret =
                    builder.Configuration["Configuration:Security:Authentication:Providers:Microsoft:ClientSecret"] ??
                    "";
                options.SaveTokens = true;
                options.CallbackPath = "/signin-microsoft";
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = builder.Configuration["Configuration:Security:Authentication:JWT:Audience"],
                    ValidIssuer = builder.Configuration["Configuration:Security:Authentication:JWT:Issuer"],
                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(
                                builder.Configuration["Configuration:Security:Authentication:JWT:Key"]!))
                };
            });

        builder.Services.AddAuthorization(cfg =>
        {
            cfg.AddPolicy("ManageUsersPolicy",
                policy => policy.Requirements.Add(new PermissionRequirement("Permission.Admin.ManageUsers")));
            cfg.AddPolicy("ManageLockoutPolicy",
                policy => policy.Requirements.Add(new PermissionRequirement("Permission.Admin.ManageLockout")));
            cfg.AddPolicy("ManageRolesPolicy",
                policy => policy.Requirements.Add(new PermissionRequirement("Permission.Admin.ManageRoles")));
            cfg.AddPolicy("ManagePermissionsPolicy",
                policy => policy.Requirements.Add(new PermissionRequirement("Permission.Admin.ManagePermissions")));
            cfg.AddPolicy("ManageAccountPolicy",
                policy => policy.Requirements.Add(new PermissionRequirement("Permission.Account.ManageAccount")));
        });
    }

    private static void AddCors(this IHostApplicationBuilder builder)
    {
        builder.Services.AddCors(o =>
        {
            o.AddDefaultPolicy(p =>
            {
                p.AllowAnyHeader();
                p.AllowAnyMethod();
                p.AllowAnyOrigin();
            });
        });
    }

    private static void AddDependencyInjection(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<ITokenHandler, TokenHandler>();
        builder.Services.AddScoped<IPermissionManager, PermissionManager>();
        builder.Services.AddScoped<ISecurityManager, SecurityManager>();
        builder.Services.AddScoped<IProfileManager, ProfileManager>();
        builder.Services.AddScoped<ICacheService, CacheService>();
        builder.Services.AddScoped<IMessageService, MessageService>();

        builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

        builder.Services.AddHostedService<HostedTokenValidator>();
        builder.Services.AddHostedService<HostedCodeValidator>();

        builder.Services.AddScoped<AppManager>();
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