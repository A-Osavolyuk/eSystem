namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Token.Validation;

public interface IJwtTokenValidationProvider
{
    public IJwtTokenValidator CreateValidator(string type);
}