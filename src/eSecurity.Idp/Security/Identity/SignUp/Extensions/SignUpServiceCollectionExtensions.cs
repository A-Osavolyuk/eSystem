using eSecurity.Idp.Security.Identity.SignUp.Strategies;

namespace eSecurity.Idp.Security.Identity.SignUp.Extensions;

public static class SignUpServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public void AddSignUpStrategies()
        {
            services.AddSingleton<ISignUpStrategyResolver, SignUpStrategyStrategyResolver>(
                sp => new SignUpStrategyStrategyResolver(sp, new Dictionary<Type, Type>()
                {
                    { typeof(ManualSignUpPayload), typeof(ManualSignUpStrategy) },
                    { typeof(OAuthSignUpPayload), typeof(OAuthSignUpStrategy) }
                }));
            
            services.AddScoped<ISignUpStrategy, ManualSignUpStrategy>();
            services.AddScoped<ISignUpStrategy, OAuthSignUpStrategy>();
        }
    }
}