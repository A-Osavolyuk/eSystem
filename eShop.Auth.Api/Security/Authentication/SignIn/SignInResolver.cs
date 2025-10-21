namespace eShop.Auth.Api.Security.Authentication.SignIn;

public class SignInResolver(IServiceProvider provider) : ISignInResolver
{
    private readonly IServiceProvider provider = provider;

    public SignInStrategy Resolve(SignInType type) => provider.GetRequiredKeyedService<SignInStrategy>(type);
}