namespace eShop.Auth.Api.Security.Authentication;

public class SignInResolver(IServiceProvider provider) : ISignInResolver
{
    private readonly IServiceProvider provider = provider;

    public SignInStrategy Resolve(SignInType type)
    {
        return type switch
        {
            SignInType.Password => provider.GetRequiredKeyedService<PasswordSignInStrategy>(type),
            SignInType.AuthenticatorApp => provider.GetRequiredKeyedService<AuthenticatorSignInStrategy>(type),
            SignInType.Passkey => provider.GetRequiredKeyedService<PasskeySignInStrategy>(type),
            SignInType.LinkedAccount => provider.GetRequiredKeyedService<LinkedAccountSignInStrategy>(type),
            _ => throw new NotSupportedException("Unknown strategy")
        };
    }
}