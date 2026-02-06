using eSecurity.Server.Security.Authorization.Constants;
using eSystem.Core.Security.Authorization.OAuth.Token.Validation;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.Validation;

public interface ITokenValidationProvider
{
    public ITokenValidator CreateValidator(TokenKind kind);
}