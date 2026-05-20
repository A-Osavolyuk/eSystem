using eSecurity.Idp.Security.Credentials.PublicKey.Credentials;
using eSecurity.Idp.Security.Authentication;
using eSecurity.Idp.Security.Authorization;
using eSecurity.Idp.Security.Credentials;
using eSecurity.Idp.Security.Cryptography;
using eSecurity.Idp.Security.Identity;
using eSystem.Core.Configuration;

namespace eSecurity.Idp.Security;

public static class SecurityExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddSecurity()
        {
            var configuration = builder.Configuration;

            builder.AddAuthentication();
            builder.AddAuthorization();
            builder.AddCryptography();
            builder.AddIdentity();
            builder.AddCredentials(cfg =>
            {
                var options = configuration.Get<CredentialOptions>("Credentials");
            
                cfg.Domain = options.Domain;
                cfg.Server = options.Server;
                cfg.Timeout = options.Timeout;
            });
        }
    }
}