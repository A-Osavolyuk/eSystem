using eSystem.Core.Security.Authentication.OpenIdConnect.Token.Validation;

namespace eSecurity.Server.Security.Authorization.Token.Validation;

public interface ITokenValidationProvider
{
    public ITokenValidator CreateValidator(string type);
}