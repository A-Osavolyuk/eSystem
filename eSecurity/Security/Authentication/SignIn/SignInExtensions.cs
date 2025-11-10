using eSecurity.Security.Authentication.SignIn.Strategies;

namespace eSecurity.Security.Authentication.SignIn;

public static class SignInExtensions
{
    public static void AddSignInStrategies(this IServiceCollection services)
    {
        services.AddScoped<ISignInResolver, SignInResolver>();
        services.AddScoped<ISignInManager, SignInManager>();
        services.AddKeyedScoped<ISignInStrategy, PasswordSignInStrategy>(SignInType.Password);
        services.AddKeyedScoped<ISignInStrategy, PasskeySignInStrategy>(SignInType.Passkey);
        services.AddKeyedScoped<ISignInStrategy, AuthenticatorSignInStrategy>(SignInType.AuthenticatorApp);
        services.AddKeyedScoped<ISignInStrategy, OAuthSignInStrategy>(SignInType.OAuth);
        services.AddKeyedScoped<ISignInStrategy, RecoveryCodeSignInStrategy>(SignInType.RecoveryCode);
    }
}