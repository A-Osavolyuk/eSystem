using eSystem.Core.Security.Authentication.SignIn;

namespace eSystem.Auth.Api.Security.Authentication.SignIn;

public interface ISignInResolver
{
    public SignInStrategy Resolve(SignInType type);
}