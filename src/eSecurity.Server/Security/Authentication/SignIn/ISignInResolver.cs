namespace eSecurity.Server.Security.Authentication.SignIn;

public interface ISignInResolver
{
    public ISignInStrategy Resolve(SignInType type);
}