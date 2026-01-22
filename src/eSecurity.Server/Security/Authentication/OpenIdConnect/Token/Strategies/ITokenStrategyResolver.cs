namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Token.Strategies;

public interface ITokenStrategyResolver
{
    public ITokenStrategy Resolve(string grantType);
}