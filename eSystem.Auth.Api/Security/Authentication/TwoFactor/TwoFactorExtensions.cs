using eSystem.Auth.Api.Security.Authentication.TwoFactor.Authenticator;
using eSystem.Auth.Api.Security.Authentication.TwoFactor.Recovery;
using eSystem.Auth.Api.Security.Authentication.TwoFactor.Secret;

namespace eSystem.Auth.Api.Security.Authentication.TwoFactor;

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