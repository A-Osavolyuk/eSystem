using eSecurity.Server.Security.Identity.SignUp.Strategies;

namespace eSecurity.Server.Security.Identity.SignUp;

public static class SignUpExtensions
{
    extension(IServiceCollection services)
    {
        public void AddSignUpStrategies()
        {
            services.AddScoped<ISignUpResolver, SignUpResolver>();
            services.AddKeyedScoped<ISignUpStrategy, ManualSignUpStrategy>(SignUpType.Manual);
            services.AddKeyedScoped<ISignUpStrategy, OAuthSignUpStrategy>(SignUpType.OAuth);
        }
    }
}