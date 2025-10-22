using eShop.Application.Common.Configuration;
using eShop.Auth.Api.Security.Authentication;
using eShop.Auth.Api.Security.Authorization;
using eShop.Auth.Api.Security.Credentials;
using eShop.Auth.Api.Security.Credentials.PublicKey;
using eShop.Auth.Api.Security.Cryptography;
using eShop.Auth.Api.Security.Identity;
using eShop.Auth.Api.Security.Tokens;

namespace eShop.Auth.Api.Security;

public static class SecurityExtensions
{
    public static void AddSecurity(this IHostApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        builder.AddAuthentication();
        builder.AddAuthorization();
        builder.AddCryptography();
        builder.AddTokens();
        builder.AddCredentials(cfg =>
        {
            var options = configuration.Get<CredentialOptions>("Configuration:Security:Credentials");
            
            cfg.Domain = options.Domain;
            cfg.Server = options.Server;
            cfg.Timeout = options.Timeout;
        });
        
        builder.AddIdentity(cfg =>
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
}