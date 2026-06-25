namespace eSecurity.Idp.Security.Authorization.Verification.Extensions;

public static class VerificationServiceCollectionExtensions
{
    public static void AddVerification(this IServiceCollection services, Action<VerificationConfiguration> configure)
    {
        services.Configure(configure);
        
        services.AddScoped<IVerificationPolicy, VerificationPolicy>();
        services.AddScoped<IVerificationQueryService, VerificationQueryService>();
        services.AddScoped<IVerificationCommandService, VerificationCommandService>();
    }
}