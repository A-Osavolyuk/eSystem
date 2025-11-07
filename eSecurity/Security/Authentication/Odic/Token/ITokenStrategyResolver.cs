namespace eSecurity.Security.Authentication.Odic.Token;

public interface ITokenStrategyResolver
{
    public ITokenStrategy Resolve(string grantType);
}