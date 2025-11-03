using eSystem.Auth.Api.Security.Authentication;
using eSystem.Auth.Api.Security.Authorization;
using eSystem.Auth.Api.Security.Credentials;
using eSystem.Auth.Api.Security.Cryptography;
using eSystem.Auth.Api.Security.Identity;
using eSystem.Auth.Api.Security.Credentials.PublicKey;
using eSystem.Auth.Api.Security.Credentials.PublicKey.Credentials;
using eSystem.Core.Common.Configuration;

namespace eSystem.Auth.Api.Security;

public static class SecurityExtensions
{
    public static void AddSecurity(this IHostApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        builder.AddAuthentication();
        builder.AddAuthorization();
        builder.AddCryptography();
        builder.AddIdentity();
        builder.AddCredentials(cfg =>
        {
            var options = configuration.Get<CredentialOptions>("Configuration:Security:Credentials");
            
            cfg.Domain = options.Domain;
            cfg.Server = options.Server;
            cfg.Timeout = options.Timeout;
        });
    }
}