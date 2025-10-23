using eSystem.Auth.Api.Security.Credentials.PublicKey;

namespace eSystem.Auth.Api.Security.Credentials;

public static class CredentialExtensions
{
    public static void AddCredentials(this IHostApplicationBuilder builder, Action<CredentialOptions> configure)
    {
        builder.Services.Configure(configure);
        builder.Services.AddScoped<IChallengeFactory, ChallengeFactory>();
        builder.Services.AddScoped<ICredentialFactory, CredentialFactory>();
    }
}