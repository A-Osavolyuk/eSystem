using eSystem.Core.Security.Authentication.SignIn;

namespace eSecurity.Security.Authentication.SignIn;

public interface ISignInResolver
{
    public SignInStrategy Resolve(SignInType type);
}