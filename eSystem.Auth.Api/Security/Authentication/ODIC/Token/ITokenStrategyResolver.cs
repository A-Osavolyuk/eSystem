namespace eSystem.Auth.Api.Security.Authentication.ODIC.Token;

public interface ITokenStrategyResolver
{
    public TokenStrategy Resolve(string grantType);
}