namespace eSecurity.Idp.Security.Identity.SignUp;

public interface ISignUpStrategyResolver
{
    ISignUpStrategy Resolve(SignUpPayload payload);
}