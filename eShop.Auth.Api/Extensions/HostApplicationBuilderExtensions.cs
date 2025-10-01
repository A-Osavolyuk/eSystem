using eShop.Application.Security.Authorization.Requirements;
using eShop.Auth.Api.Messages.Email;
using eShop.Auth.Api.Messages.Sms;
using eShop.Auth.Api.Security.Hashing;
using eShop.Auth.Api.Security.Protection;
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
        builder.AddMessaging();
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
        
        builder.Services.AddControllers()
            .AddJsonOptions(cfg => cfg.JsonSerializerOptions.WriteIndented = true);
        
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddDistributedMemoryCache();
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

        builder.Services.Configure<JwtOptions>(configuration.GetSection("Jwt"));

        builder.Services.AddAuthorization();
        builder.Services.AddEncryption();
        builder.Services.AddHashing();

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
    }

    private static IServiceCollection AddIdentity(this IServiceCollection services,
        Action<IdentityOptions> configureOptions)
    {
        var options = new IdentityOptions();
        configureOptions(options);

        services.AddSingleton(options);

        return services;
    }

    private static IServiceCollection AddEncryption(this IServiceCollection services)
    {
        services.AddDataProtection();
        services.AddSingleton<SecretProtector>();

        return services;
    }

    private static IServiceCollection AddHashing(this IServiceCollection services)
    {
        services.AddScoped<Hasher, Pbkdf2Hasher>();

        return services;
    }

    private static IServiceCollection AddAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy("DeleteAccountPolicy", policy => policy.Requirements.Add(new PermissionRequirement("DELETE_ACCOUNT")))
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
            .AddPolicy("ReadPermissionPolicy", policy => policy.Requirements.Add(new PermissionRequirement("READ_PERMISSIONS")))
            .AddPolicy("GrantPermissionPolicy", policy => policy.Requirements.Add(new PermissionRequirement("GRANT_PERMISSIONS")))
            .AddPolicy("RevokePermissionPolicy", policy => policy.Requirements.Add(new PermissionRequirement("REVOKE_PERMISSIONS")));

        return services;
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

    private static void AddMessaging(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMessaging(cfg =>
        {
            cfg.Add<VerifyEmailMessage>(SenderType.Email, CodeResource.Email, CodeType.Verify);
            cfg.Add<RemoveEmailMessage>(SenderType.Email, CodeResource.Email, CodeType.Remove);
            cfg.Add<ChangeEmailMessage>(SenderType.Email, CodeResource.Email, CodeType.Current);
            cfg.Add<ResetEmailMessage>(SenderType.Email, CodeResource.Email, CodeType.Reset);
            cfg.Add<ConfirmEmailChangeMessage>(SenderType.Email, CodeResource.Email, CodeType.New);
            cfg.Add<ManageEmailMessage>(SenderType.Email, CodeResource.Email, CodeType.Manage);
            cfg.Add<UnblockAccountMessage>(SenderType.Email, CodeResource.Account, CodeType.Unlock);
            cfg.Add<RecoverAccountMessage>(SenderType.Email, CodeResource.Account, CodeType.Recover);
            cfg.Add<AllowLinkedAccountMessage>(SenderType.Email, CodeResource.LinkedAccount, CodeType.Allow);
            cfg.Add<DisallowLinkedAccountMessage>(SenderType.Email, CodeResource.LinkedAccount, CodeType.Disallow);
            cfg.Add<DisconnectLinkedAccountMessage>(SenderType.Email, CodeResource.LinkedAccount, CodeType.Disconnect);
            cfg.Add<ForgotPasswordMessage>(SenderType.Email, CodeResource.Password, CodeType.Reset);
            cfg.Add<RemovePasskeyMessage>(SenderType.Email, CodeResource.Passkey, CodeType.Remove);
            cfg.Add<BlockDeviceMessage>(SenderType.Email, CodeResource.Device, CodeType.Block);
            cfg.Add<TrustDeviceMessage>(SenderType.Email, CodeResource.Device, CodeType.Trust);
            cfg.Add<UnblockDeviceMessage>(SenderType.Email, CodeResource.Device, CodeType.Unblock);
            cfg.Add<VerifyDeviceMessage>(SenderType.Email, CodeResource.Device, CodeType.Verify);
            cfg.Add<TwoFactorCodeEmailMessage>(SenderType.Email, CodeResource.TwoFactor, CodeType.SignIn);
            cfg.Add<EnableEmailTwoFactorMessage>(SenderType.Email, CodeResource.Provider, CodeType.Subscribe);
            cfg.Add<DisableEmailTwoFactorMessage>(SenderType.Email, CodeResource.Provider, CodeType.Unsubscribe);
            cfg.Add<EnableMethodMessage>(SenderType.Email, CodeResource.LoginMethod, CodeType.Enable);
            cfg.Add<DisableMethodMessage>(SenderType.Email, CodeResource.LoginMethod, CodeType.Disable);
            cfg.Add<ChangePhoneNumberMessage>(SenderType.Sms, CodeResource.PhoneNumber, CodeType.Current);
            cfg.Add<RemovePhoneNumberMessage>(SenderType.Sms, CodeResource.PhoneNumber, CodeType.Remove);
            cfg.Add<ResetPhoneNumberMessage>(SenderType.Sms, CodeResource.PhoneNumber, CodeType.Reset);
            cfg.Add<VerifyPhoneNumberMessage>(SenderType.Sms, CodeResource.PhoneNumber, CodeType.Verify);
            cfg.Add<ConfirmPhoneNumberChangeMessage>(SenderType.Sms, CodeResource.PhoneNumber, CodeType.New);
            cfg.Add<TwoFactorCodeSmsMessage>(SenderType.Sms, CodeResource.TwoFactor, CodeType.SignIn);
            cfg.Add<EnableSmsTwoFactorMessage>(SenderType.Sms, CodeResource.Provider, CodeType.Subscribe);
        });
    }
}