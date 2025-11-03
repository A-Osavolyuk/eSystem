using eSecurity.Security.Authentication.TwoFactor.Authenticator;
using eSecurity.Security.Authentication.TwoFactor.Recovery;
using eSecurity.Security.Authentication.TwoFactor.Secret;

namespace eSecurity.Security.Authentication.TwoFactor;

public static class TwoFactorExtensions
{
    public static void Add2FA(this IServiceCollection services)
    {
        services.AddScoped<IQrCodeFactory, QrCodeFactory>();
        services.AddScoped<IRecoveryCodeFactory, RecoveryCodeFactory>();
        services.AddScoped<IRecoverManager, RecoverManager>();
        services.AddScoped<ISecretManager, SecretManager>();
        services.AddScoped<ITwoFactorManager, TwoFactorManager>();
    }
}