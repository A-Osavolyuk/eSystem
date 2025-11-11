using eSecurity.Server.Security.Authorization.OAuth.LinkedAccount;

namespace eSecurity.Server.Security.Authorization.OAuth;

public static class OAuthExtensions
{
    public static void AddOAuthAuthorization(this IServiceCollection services)
    {
        services.AddScoped<IOAuthSessionManager, OAuthSessionManager>();
        services.AddScoped<ILinkedAccountManager, LinkedAccountManager>();
    }
}