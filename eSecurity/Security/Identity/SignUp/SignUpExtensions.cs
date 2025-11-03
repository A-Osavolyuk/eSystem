using eSecurity.Security.Identity.SignUp.Strategies;

namespace eSecurity.Security.Identity.SignUp;

public static class SignUpExtensions
{
    public static void AddSignUpStrategies(this IServiceCollection services)
    {
        services.AddScoped<ISignUpResolver, SignUpResolver>();
        services.AddKeyedScoped<SignUpStrategy, ManualSignUpStrategy>(SignUpType.Manual);
        services.AddKeyedScoped<SignUpStrategy, OAuthSignUpStrategy>(SignUpType.OAuth);
    }
}