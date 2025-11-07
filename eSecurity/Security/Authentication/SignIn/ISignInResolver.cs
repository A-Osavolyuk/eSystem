namespace eSecurity.Security.Authentication.SignIn;

public interface ISignInResolver
{
    public ISignInStrategy Resolve(SignInType type);
}