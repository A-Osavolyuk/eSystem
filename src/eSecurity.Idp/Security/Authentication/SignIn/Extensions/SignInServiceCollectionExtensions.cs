using eSecurity.Idp.Security.Authentication.SignIn.Strategies;

namespace eSecurity.Idp.Security.Authentication.SignIn.Extensions;

public static class SignInServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public void AddSignInStrategies()
        {
            services.AddSingleton<ISignInStrategyResolver, SignInStrategyResolver>();
            services.AddScoped<ISignInStrategy, PasswordSignInStrategy>();
            services.AddScoped<ISignInStrategy, SoftwareKeySignInStrategy>();
            services.AddScoped<ISignInStrategy, OAuthSignInStrategy>();
            services.AddScoped<ISignInStrategy, TwoFactorSignInStrategy>();
        }
    }
}