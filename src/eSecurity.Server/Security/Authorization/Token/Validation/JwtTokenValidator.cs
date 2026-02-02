using System.IdentityModel.Tokens.Jwt;
using eSystem.Core.Security.Authentication.OpenIdConnect.Token.Validation;
using TokenValidationResult = eSystem.Core.Security.Authentication.OpenIdConnect.Token.Validation.TokenValidationResult;

namespace eSecurity.Server.Security.Authorization.Token.Validation;

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
        return await validator.ValidateAsync(token, cancellationToken);
    }
}