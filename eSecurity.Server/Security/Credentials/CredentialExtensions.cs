using eSecurity.Server.Security.Credentials.PublicKey;
using eSecurity.Server.Security.Credentials.PublicKey.Challenge;
using eSecurity.Server.Security.Credentials.PublicKey.Credentials;

namespace eSecurity.Server.Security.Credentials;

public static class CredentialExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddCredentials(Action<CredentialOptions> configure)
        {
            builder.Services.Configure(configure);
            builder.Services.AddScoped<IChallengeFactory, ChallengeFactory>();
            builder.Services.AddScoped<ICredentialFactory, CredentialFactory>();
            builder.Services.AddScoped<IPasskeyManager, PasskeyManager>();
        }
    }
}