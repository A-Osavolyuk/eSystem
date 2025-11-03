namespace eSecurity.Security.Identity.SignUp;

public class SignUpResolver(IServiceProvider serviceProvider) : ISignUpResolver
{
    private readonly IServiceProvider serviceProvider = serviceProvider;

    public SignUpStrategy Resolve(SignUpType type) => serviceProvider.GetRequiredKeyedService<SignUpStrategy>(type);
}