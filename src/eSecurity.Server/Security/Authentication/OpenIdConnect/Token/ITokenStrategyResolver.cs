namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Token;

public interface ITokenStrategyResolver
{
    public ITokenStrategy Resolve(string grantType);
}