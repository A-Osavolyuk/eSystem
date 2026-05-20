using eSecurity.Idp.Security.Authentication.SignIn.Strategies;

namespace eSecurity.Idp.Security.Authentication.SignIn;

public static class SignInExtensions
{
    extension(IServiceCollection services)
    {
        public void AddSignInStrategies()
        {
            services.AddScoped<ISignInResolver, SignInResolver>();
            services.AddKeyedScoped<ISignInStrategy, PasswordSignInStrategy>(SignInType.Password);
            services.AddKeyedScoped<ISignInStrategy, PasskeySignInStrategy>(SignInType.Passkey);
            services.AddKeyedScoped<ISignInStrategy, OAuthSignInStrategy>(SignInType.OAuth);
            services.AddKeyedScoped<ISignInStrategy, TwoFactorSignInStrategy>(SignInType.TwoFactor);
        }
    }
}