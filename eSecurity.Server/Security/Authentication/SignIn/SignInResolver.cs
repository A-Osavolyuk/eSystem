namespace eSecurity.Server.Security.Authentication.SignIn;

public class SignInResolver(IServiceProvider provider) : ISignInResolver
{
    private readonly IServiceProvider provider = provider;

    public ISignInStrategy Resolve(SignInType type) => provider.GetRequiredKeyedService<ISignInStrategy>(type);
}