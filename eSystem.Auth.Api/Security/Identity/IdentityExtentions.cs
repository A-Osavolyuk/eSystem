using eSystem.Auth.Api.Security.Identity.Options;
using eSystem.Auth.Api.Security.Identity.Privacy;
using eSystem.Auth.Api.Security.Identity.SignUp;
using eSystem.Auth.Api.Security.Identity.User;

namespace eSystem.Auth.Api.Security.Identity;

public static class IdentityExtensions
{
    public static void AddIdentity(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSignUpStrategies();
        builder.Services.AddScoped<IUserManager, UserManager>();
        builder.Services.AddScoped<IPersonalDataManager, PersonalDataManager>();
        
        builder.ConfigureIdentity(cfg =>
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
    }
    
    private static void ConfigureIdentity(this IHostApplicationBuilder builder, Action<IdentityBuilder> configurator)
    {
        var identityBuilder = new IdentityBuilder(builder.Services);
        configurator(identityBuilder);
        
        builder.Services.AddScoped<IUserManager, UserManager>();
        builder.Services.AddScoped<IPersonalDataManager, PersonalDataManager>();
    }
}


public class IdentityBuilder(IServiceCollection services)
{
    public IServiceCollection Services { get; } = services;
    public void ConfigurePassword(Action<PasswordOptions> configure) => Services.Configure(configure);
    public void ConfigureAccount(Action<AccountOptions> configure) => Services.Configure(configure);
    public void ConfigureSignIn(Action<SignInOptions> configure) => Services.Configure(configure);
    public void ConfigureCode(Action<CodeOptions> configure) => Services.Configure(configure);
}