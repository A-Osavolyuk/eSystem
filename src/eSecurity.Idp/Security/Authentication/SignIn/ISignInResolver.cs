namespace eSecurity.Idp.Security.Authentication.SignIn;

public interface ISignInResolver
{
    public ISignInStrategy Resolve(SignInType type);
}