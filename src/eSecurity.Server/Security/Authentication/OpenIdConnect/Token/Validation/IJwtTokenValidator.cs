using System.IdentityModel.Tokens.Jwt;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Token.Validation;

public interface IJwtTokenValidator
{
    public Task<TokenValidationResult> ValidateAsync(JwtSecurityToken token, 
        CancellationToken cancellationToken = default);
}