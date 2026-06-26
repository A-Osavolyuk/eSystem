using eSecurity.Core.Security.Authentication.SignIn;

namespace eSecurity.Idp.Security.Authentication.SignIn;

public interface ISignInStrategyResolver
{
    ISignInStrategy Resolve(SignInPayload payload);
}