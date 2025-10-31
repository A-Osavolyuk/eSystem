using eSystem.Auth.Api.Security.Identity.SignUp.Strategies;

namespace eSystem.Auth.Api.Security.Identity.SignUp;

public static class SignUpExtensions
{
    public static void AddSignUpStrategies(this IServiceCollection services)
    {
        services.AddScoped<ISignUpResolver, SignUpResolver>();
        services.AddKeyedScoped<SignUpStrategy, ManualSignUpStrategy>(SignUpType.Manual);
        services.AddKeyedScoped<SignUpStrategy, OAuthSignUpStrategy>(SignUpType.OAuth);
    }
}