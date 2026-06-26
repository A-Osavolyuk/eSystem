using eSecurity.Idp.Security.Authentication.TwoFactor.AuthenticatorApp;
using eSecurity.Idp.Security.Authentication.TwoFactor.RecoveryCode;
using eSecurity.Idp.Security.Authentication.TwoFactor.Secret;
using eSecurity.Idp.Security.Authentication.TwoFactor.SoftwareKey;

namespace eSecurity.Idp.Security.Authentication.TwoFactor.Extensions;

public static class TwoFactorServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public void Add2Fa()
        {
            services.AddSingleton<ITwoFactorStrategyResolver, TwoFactorStrategyResolver>();
            services.AddScoped<IQrCodeFactory, QrCodeFactory>();
            services.AddScoped<IRecoveryCodeFactory, RecoveryCodeFactory>();
            services.AddScoped<IRecoverManager, RecoverManager>();
            services.AddScoped<ISecretManager, SecretManager>();
            services.AddScoped<ITwoFactorQueryService, TwoFactorQueryService>();
            services.AddScoped<ITwoFactorCommandService, TwoFactorCommandService>();
            services.AddScoped<ITwoFactorPolicy, TwoFactorPolicy>();
            services.AddScoped<ITwoFactorContextMapper, TwoFactorContextMapper>();
            services.AddScoped<ITwoFactorStrategy<AuthenticatorTwoFactorContext>, AuthenticatorAppTwoFactorStrategy>();
            services.AddScoped<ITwoFactorStrategy<SoftwareKeyTwoFactorContext>, SoftwareKeyTwoFactorStrategy>();
            services.AddScoped<ITwoFactorStrategy<RecoveryCodeTwoFactorContext>, RecoveryCodeTwoFactorStrategy>();
        }
    }
}