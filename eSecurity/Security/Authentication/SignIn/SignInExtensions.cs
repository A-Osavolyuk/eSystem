using eSecurity.Security.Authentication.SignIn.Strategies;
using eSystem.Core.Security.Authentication.SignIn;

namespace eSecurity.Security.Authentication.SignIn;

public static class SignInExtensions
{
    public static void AddSignInStrategies(this IServiceCollection services)
    {
        services.AddScoped<ISignInResolver, SignInResolver>();
        services.AddScoped<ISignInManager, SignInManager>();
        services.AddKeyedScoped<SignInStrategy, PasswordSignInStrategy>(SignInType.Password);
        services.AddKeyedScoped<SignInStrategy, PasskeySignInStrategy>(SignInType.Passkey);
        services.AddKeyedScoped<SignInStrategy, AuthenticatorSignInStrategy>(SignInType.AuthenticatorApp);
        services.AddKeyedScoped<SignInStrategy, OAuthSignInStrategy>(SignInType.OAuth);
    }
}