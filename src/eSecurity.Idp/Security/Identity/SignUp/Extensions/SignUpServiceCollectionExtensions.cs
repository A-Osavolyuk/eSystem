using eSecurity.Idp.Security.Identity.SignUp.Strategies;

namespace eSecurity.Idp.Security.Identity.SignUp.Extensions;

public static class SignUpServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public void AddSignUpStrategies()
        {
            services.AddSingleton<ISignUpStrategyResolver, SignUpStrategyStrategyResolver>();
            services.AddScoped<ISignUpStrategy, ManualSignUpStrategy>();
            services.AddScoped<ISignUpStrategy, OAuthSignUpStrategy>();
        }
    }
}