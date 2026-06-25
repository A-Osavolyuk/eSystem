using eSecurity.Idp.Security.Authentication.TwoFactor.AuthenticatorApp;
using eSecurity.Idp.Security.Authentication.TwoFactor.Passkey;
using eSecurity.Idp.Security.Authentication.TwoFactor.RecoveryCode;
using eSecurity.Idp.Security.Authentication.TwoFactor.Secret;

namespace eSecurity.Idp.Security.Authentication.TwoFactor.Extensions;

public static class TwoFactorServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public void Add2Fa()
        {
            services.AddScoped<IQrCodeFactory, QrCodeFactory>();
            services.AddScoped<IRecoveryCodeFactory, RecoveryCodeFactory>();
            services.AddScoped<IRecoverManager, RecoverManager>();
            services.AddScoped<ISecretManager, SecretManager>();
            services.AddScoped<ITwoFactorQueryService, TwoFactorQueryService>();
            services.AddScoped<ITwoFactorCommandService, TwoFactorCommandService>();
            services.AddScoped<ITwoFactorPolicy, TwoFactorPolicy>();
            services.AddScoped<ITwoFactorContextMapper, TwoFactorContextMapper>();
            services.AddScoped<ITwoFactorStrategyResolver, TwoFactorStrategyResolver>();
            services.AddScoped<ITwoFactorStrategy<AuthenticatorTwoFactorContext>, AuthenticatorAppTwoFactorStrategy>();
            services.AddScoped<ITwoFactorStrategy<PasskeyTwoFactorContext>, SoftwareKeyTwoFactorStrategy>();
            services.AddScoped<ITwoFactorStrategy<RecoveryCodeTwoFactorContext>, RecoveryCodeTwoFactorStrategy>();
        }
    }
}