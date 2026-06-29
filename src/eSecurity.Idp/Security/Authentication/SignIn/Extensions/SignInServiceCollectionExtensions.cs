using eSecurity.Core.Security.Authentication.SignIn;
using eSecurity.Idp.Security.Authentication.SignIn.Strategies;

namespace eSecurity.Idp.Security.Authentication.SignIn.Extensions;

public static class SignInServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public void AddSignInStrategies()
        {
            services.AddSingleton<ISignInStrategyResolver>(
                sp => new SignInStrategyResolver(sp, new Dictionary<Type, Type>
                {
                    { typeof(PasswordSignInPayload), typeof(PasswordSignInStrategy) },
                    { typeof(OAuthSignInPayload), typeof(OAuthSignInStrategy) },
                    { typeof(SoftwareKeySignInPayload), typeof(SoftwareKeySignInStrategy) },
                    { typeof(TwoFactorSignInPayload), typeof(TwoFactorSignInPayload) },
                }));

            services.AddScoped<ISignInStrategy, PasswordSignInStrategy>();
            services.AddScoped<ISignInStrategy, SoftwareKeySignInStrategy>();
            services.AddScoped<ISignInStrategy, OAuthSignInStrategy>();
            services.AddScoped<ISignInStrategy, TwoFactorSignInStrategy>();
        }
    }
}