namespace eSecurity.Security.Identity.SignUp;

public class SignUpResolver(IServiceProvider serviceProvider) : ISignUpResolver
{
    private readonly IServiceProvider serviceProvider = serviceProvider;

    public ISignUpStrategy Resolve(SignUpType type) => serviceProvider.GetRequiredKeyedService<ISignUpStrategy>(type);
}