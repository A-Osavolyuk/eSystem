using eSystem.Core.Security.Authorization.OAuth;

namespace eSecurity.Idp.Security.Authorization.OAuth.Token.Validation;

public interface IJwtTokenValidationProvider
{
    public IJwtTokenValidator CreateValidator(JwtTokenType type);
}