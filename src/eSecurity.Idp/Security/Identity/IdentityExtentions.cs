using eSecurity.Idp.Security.Identity.Email;
using eSecurity.Idp.Security.Identity.Options;
using eSecurity.Idp.Security.Identity.Privacy;
using eSecurity.Idp.Security.Identity.User;
using eSecurity.Idp.Security.Identity.User.Username;
using eSecurity.Idp.Security.Identity.SignUp.Extensions;
using eSecurity.Idp.Security.Identity.Subject;

namespace eSecurity.Idp.Security.Identity;

public static class IdentityExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddIdentity()
        {
            builder.Services.AddSignUpStrategies();
            builder.Services.AddScoped<IUsernameManager, UsernameManager>();
            builder.Services.AddScoped<IPersonalDataManager, PersonalDataManager>();
            builder.Services.AddScoped<IPairwiseSubjectFactory, PairwiseSubjectFactory>();

            builder.Services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();
            builder.Services.AddScoped<IUserQueryService, UserQueryService>();
            builder.Services.AddScoped<IUserCommandService, UserCommandService>();
            builder.Services.AddScoped<IUserFailedLoginService, UserFailedLoginService>();
            builder.Services.AddScoped<IUserResendAttemptsService, UserResendAttemptsService>();
            
            builder.Services.AddScoped<IEmailQueryService, EmailQueryService>();
            builder.Services.AddScoped<IEmailCommandService, EmailCommandService>();
            builder.Services.AddScoped<IEmailPolicy, EmailPolicy>();
        
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

        private void ConfigureIdentity(Action<IdentityBuilder> configurator)
        {
            var identityBuilder = new IdentityBuilder(builder.Services);
            configurator(identityBuilder);
            
            builder.Services.AddScoped<IPersonalDataManager, PersonalDataManager>();
        }
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