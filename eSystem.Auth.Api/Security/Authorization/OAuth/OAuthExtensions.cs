namespace eSystem.Auth.Api.Security.Authorization.OAuth;

public static class OAuthExtensions
{
    public static void AddOAuthAuthorization(this IServiceCollection services)
    {
        services.AddScoped<IOAuthSessionManager, OAuthSessionManager>();
        services.AddScoped<ILinkedAccountManager, LinkedAccountManager>();
    }
}