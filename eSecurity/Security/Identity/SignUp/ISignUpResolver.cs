namespace eSecurity.Security.Identity.SignUp;

public interface ISignUpResolver
{
    public ISignUpStrategy Resolve(SignUpType type);
}