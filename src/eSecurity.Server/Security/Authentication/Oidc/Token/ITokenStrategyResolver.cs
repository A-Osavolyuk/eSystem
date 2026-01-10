namespace eSecurity.Server.Security.Authentication.Oidc.Token;

public interface ITokenStrategyResolver
{
    public ITokenStrategy Resolve(string grantType);
}