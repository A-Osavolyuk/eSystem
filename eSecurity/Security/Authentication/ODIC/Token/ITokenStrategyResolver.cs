namespace eSecurity.Security.Authentication.ODIC.Token;

public interface ITokenStrategyResolver
{
    public TokenStrategy Resolve(string grantType);
}