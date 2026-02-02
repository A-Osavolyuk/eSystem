namespace eSecurity.Server.Security.Authorization.Token.Strategies;

public interface ITokenStrategyResolver
{
    public ITokenStrategy Resolve(string grantType);
}