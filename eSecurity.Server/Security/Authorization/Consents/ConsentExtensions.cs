namespace eSecurity.Server.Security.Authorization.Consents;

public static class ConsentExtensions
{
    public static void AddConsentManagement(this IServiceCollection services)
    {
        services.AddScoped<IConsentManager, ConsentManager>();
    }
}