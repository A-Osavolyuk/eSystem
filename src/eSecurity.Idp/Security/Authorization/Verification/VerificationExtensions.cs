using eSecurity.Idp.Security.Authorization.Verification.AuthenticationApp;
using eSecurity.Idp.Security.Authorization.Verification.Passkey;
using eSecurity.Idp.Security.Authorization.Verification.Totp;

namespace eSecurity.Idp.Security.Authorization.Verification;

public static class VerificationExtensions
{
    public static void AddVerification(this IServiceCollection services, Action<VerificationConfiguration> configure)
    {
        services.Configure(configure);
        
        services.AddScoped<IVerificationManager, VerificationManager>();
        services.AddScoped<IVerificationStrategyResolver, VerificationStrategyResolver>();
        services.AddScoped<IVerificationStrategy<TotpVerificationContext>, TotpVerificationStrategy>();
        services.AddScoped<IVerificationStrategy<PasskeyVerificationContext>, PasskeyVerificationStrategy>();
        services.AddScoped<IVerificationStrategy<AuthenticatorAppVerificationContext>, AuthenticationAppVerificationStrategy>();
    }
}