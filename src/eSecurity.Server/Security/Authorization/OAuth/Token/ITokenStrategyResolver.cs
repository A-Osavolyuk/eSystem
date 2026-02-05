namespace eSecurity.Server.Security.Authorization.OAuth.Token;

public interface ITokenStrategyResolver
{
    public ITokenStrategy Resolve(string grantType);
}