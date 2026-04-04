using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Security.Authorization.OAuth.Constants;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.Validation;

public interface IJwtTokenValidationProvider
{
    public IJwtTokenValidator CreateValidator(JwtTokenType type);
}