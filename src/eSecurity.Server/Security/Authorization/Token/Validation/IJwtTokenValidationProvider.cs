namespace eSecurity.Server.Security.Authorization.Token.Validation;

public interface IJwtTokenValidationProvider
{
    public IJwtTokenValidator CreateValidator(string type);
}