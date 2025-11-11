using eSecurity.Server.Security.Identity.SignUp.Strategies;

namespace eSecurity.Server.Security.Identity.SignUp;

public static class SignUpExtensions
{
    public static void AddSignUpStrategies(this IServiceCollection services)
    {
        services.AddScoped<ISignUpResolver, SignUpResolver>();
        services.AddKeyedScoped<ISignUpStrategy, ManualSignUpStrategy>(SignUpType.Manual);
        services.AddKeyedScoped<ISignUpStrategy, OAuthSignUpStrategy>(SignUpType.OAuth);
    }
}