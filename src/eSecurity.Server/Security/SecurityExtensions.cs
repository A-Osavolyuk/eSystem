using eSecurity.Core.Security.Cookies;
using eSecurity.Server.Security.Authentication;
using eSecurity.Server.Security.Authorization;
using eSecurity.Server.Security.Credentials;
using eSecurity.Server.Security.Credentials.PublicKey.Credentials;
using eSecurity.Server.Security.Cryptography;
using eSecurity.Server.Security.Identity;
using eSystem.Core.Common.Configuration;

namespace eSecurity.Server.Security;

public static class SecurityExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddSecurity()
        {
            var configuration = builder.Configuration;
            builder.Services.AddCookies();

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