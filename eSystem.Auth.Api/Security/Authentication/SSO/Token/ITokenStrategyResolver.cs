namespace eSystem.Auth.Api.Security.Authentication.SSO.Token;

public interface ITokenStrategyResolver
{
    public TokenStrategy Resolve(string grantType);
}