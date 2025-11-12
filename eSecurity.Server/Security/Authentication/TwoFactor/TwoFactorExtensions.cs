using eSecurity.Server.Security.Authentication.TwoFactor.Authenticator;
using eSecurity.Server.Security.Authentication.TwoFactor.Recovery;
using eSecurity.Server.Security.Authentication.TwoFactor.Secret;

namespace eSecurity.Server.Security.Authentication.TwoFactor;

public static class TwoFactorExtensions
{
    extension(IServiceCollection services)
    {
        public void Add2Fa()
        {
            services.AddScoped<IQrCodeFactory, QrCodeFactory>();
            services.AddScoped<IRecoveryCodeFactory, RecoveryCodeFactory>();
            services.AddScoped<IRecoverManager, RecoverManager>();
            services.AddScoped<ISecretManager, SecretManager>();
            services.AddScoped<ITwoFactorManager, TwoFactorManager>();
        }
    }
}