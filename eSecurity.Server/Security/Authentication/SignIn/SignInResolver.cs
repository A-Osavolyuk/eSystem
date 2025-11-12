namespace eSecurity.Server.Security.Authentication.SignIn;

public class SignInResolver(IServiceProvider provider) : ISignInResolver
{
    private readonly IServiceProvider _provider = provider;

    public ISignInStrategy Resolve(SignInType type) => _provider.GetRequiredKeyedService<ISignInStrategy>(type);
}