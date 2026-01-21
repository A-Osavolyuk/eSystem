namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Token.Validation;

public interface ITokenValidationProvider
{
    public ITokenValidator CreateValidator(string type);
}