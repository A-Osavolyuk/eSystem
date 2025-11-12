namespace eSecurity.Server.Security.Identity.SignUp;

public class SignUpResolver(IServiceProvider serviceProvider) : ISignUpResolver
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public ISignUpStrategy Resolve(SignUpType type) => _serviceProvider.GetRequiredKeyedService<ISignUpStrategy>(type);
}