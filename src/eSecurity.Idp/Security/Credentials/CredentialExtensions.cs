using eSecurity.Idp.Security.Credentials.PublicKey;
using eSecurity.Idp.Security.Credentials.PublicKey.Challenge;
using eSecurity.Idp.Security.Credentials.PublicKey.Credentials;

namespace eSecurity.Idp.Security.Credentials;

public static class CredentialExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddCredentials(Action<CredentialOptions> configure)
        {
            builder.Services.Configure(configure);
            builder.Services.AddScoped<ISoftwareKeyQueryService, SoftwareKeyQueryService>();
            builder.Services.AddScoped<ISoftwareKeyCommandService, SoftwareKeyCommandService>();
        }
    }
}