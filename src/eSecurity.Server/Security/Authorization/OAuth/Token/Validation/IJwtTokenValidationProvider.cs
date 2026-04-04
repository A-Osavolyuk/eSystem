using eSystem.Core.Security.Authorization.OAuth;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.Validation;

public interface IJwtTokenValidationProvider
{
    public IJwtTokenValidator CreateValidator(JwtTokenType type);
}