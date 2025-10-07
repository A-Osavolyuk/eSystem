namespace eShop.Auth.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddIdentity(this IServiceCollection services,
        Action<IdentityOptions> configureOptions)
    {
        var options = new IdentityOptions();
        configureOptions(options);

        services.AddSingleton(options);
    }
    
    public static void AddVerification(this IServiceCollection services, Action<VerificationOptions> configure)
    {
        var options = new VerificationOptions();
        configure(options);
        
        services.AddSingleton(options);
    }

    public static void Add2FA(this IServiceCollection services, Action<TwoFactorOptions> configure)
    {
        var options = new TwoFactorOptions();
        configure(options);
        
        services.AddSingleton(options);
    }
}