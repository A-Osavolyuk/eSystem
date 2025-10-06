namespace eShop.Auth.Api.Extensions;

public static class VerificationServiceCollectionExtensions
{
    public static void AddVerification(this IServiceCollection services, Action<VerificationOptions> configure)
    {
        var options = new VerificationOptions();
        configure(options);
        
        services.AddSingleton(options);
        
    }
}