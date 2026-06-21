namespace eSecurity.Idp.Security.Authorization.Verification;

public static class VerificationExtensions
{
    public static void AddVerification(this IServiceCollection services, Action<VerificationConfiguration> configure)
    {
        services.Configure(configure);
        
        services.AddScoped<IVerificationPolicy, VerificationPolicy>();
        services.AddScoped<IVerificationQueryService, VerificationQueryService>();
        services.AddScoped<IVerificationCommandService, VerificationCommandService>();
    }
}