using eSecurity.Security.Credentials.PublicKey.Credentials;
using eSecurity.Security.Authentication;
using eSecurity.Security.Authorization;
using eSecurity.Security.Credentials;
using eSecurity.Security.Cryptography;
using eSecurity.Security.Identity;
using eSecurity.Security.Credentials.PublicKey;
using eSystem.Core.Common.Configuration;

namespace eSecurity.Security;

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