namespace eShop.Auth.Api.Security.Authentication;

public interface ISignInResolver
{
    public SignInStrategy Resolve(SignInType type);
}