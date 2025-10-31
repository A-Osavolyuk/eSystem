namespace eSystem.Auth.Api.Security.Identity.SignUp;

public interface ISignUpResolver
{
    public SignUpStrategy Resolve(SignUpType type);
}