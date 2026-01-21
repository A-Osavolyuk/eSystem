using System.IdentityModel.Tokens.Jwt;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Token.Validation;

public class JwtTokenValidator(IJwtTokenValidationProvider validationProvider) : ITokenValidator
{
    private readonly IJwtTokenValidationProvider _validationProvider = validationProvider;
    private readonly JwtSecurityTokenHandler _handler = new();

    public async Task<TokenValidationResult> ValidateAsync(string token, CancellationToken cancellationToken = default)
    {
        if (!_handler.CanReadToken(token))
            return TokenValidationResult.Fail();
        
        var securityToken = _handler.ReadJwtToken(token);
        if (securityToken is null)
            return TokenValidationResult.Fail();
        
        var validator = _validationProvider.CreateValidator(securityToken.Header.Typ);
        return await validator.ValidateAsync(securityToken, cancellationToken);
    }
}