namespace eSecurity.Security.Authentication.Odic.Token;

public interface ITokenStrategyResolver
{
    public TokenStrategy Resolve(string grantType);
}