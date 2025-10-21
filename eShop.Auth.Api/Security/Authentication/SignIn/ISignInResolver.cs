namespace eShop.Auth.Api.Security.Authentication.SignIn;

public interface ISignInResolver
{
    public SignInStrategy Resolve(SignInType type);
}