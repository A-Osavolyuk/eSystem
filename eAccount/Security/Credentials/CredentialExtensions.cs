using eAccount.Security.Credentials.PublicKey;

namespace eAccount.Security.Credentials;

public static class CredentialExtensions
{
    public static void AddCredentials(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<PasskeyManager>();
        builder.Services.AddScoped<IPasskeyService, PasskeyService>();
    }
}