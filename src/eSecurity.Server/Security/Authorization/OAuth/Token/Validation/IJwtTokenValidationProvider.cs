namespace eSecurity.Server.Security.Authorization.OAuth.Token.Validation;

public interface IJwtTokenValidationProvider
{
    public IJwtTokenValidator CreateValidator(string type);
}