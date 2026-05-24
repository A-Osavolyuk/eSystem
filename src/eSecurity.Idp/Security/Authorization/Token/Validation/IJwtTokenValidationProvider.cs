using eSystem.Core.Security.Authorization.OAuth;

namespace eSecurity.Idp.Security.Authorization.Token.Validation;

public interface IJwtTokenValidationProvider
{
    public IJwtTokenValidator CreateValidator(JwtTokenType type);
}