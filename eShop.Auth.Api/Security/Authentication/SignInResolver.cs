namespace eShop.Auth.Api.Security.Authentication;

public class SignInResolver(IServiceProvider provider) : ISignInResolver
{
    private readonly IServiceProvider provider = provider;

    public SignInStrategy Resolve(SignInType type) => provider.GetRequiredKeyedService<SignInStrategy>(type);
}