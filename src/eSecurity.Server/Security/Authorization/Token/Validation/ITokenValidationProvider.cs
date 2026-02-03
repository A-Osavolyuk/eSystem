using eSystem.Core.Security.Authorization.OAuth.Token.Validation;

namespace eSecurity.Server.Security.Authorization.Token.Validation;

public interface ITokenValidationProvider
{
    public ITokenValidator CreateValidator(string type);
}