using System.IdentityModel.Tokens.Jwt;
using eSystem.Core.Enums;
using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Server.Security.Authorization.OAuth.Token.Validation;
using TokenValidationResult = eSystem.Core.Server.Security.Authorization.OAuth.Token.Validation.TokenValidationResult;

namespace eSecurity.Idp.Security.Authorization.Token.Validation;

public class JwtTokenValidator(IJwtTokenValidationProvider validationProvider) : ITokenValidator
{
    private readonly IJwtTokenValidationProvider _validationProvider = validationProvider;
    private readonly JwtSecurityTokenHandler _handler = new();

    public async Task<TokenValidationResult> ValidateAsync(string token, CancellationToken cancellationToken = default)
    {
        if (!_handler.CanReadToken(token))
            return TokenValidationResult.Fail();
        
        var securityToken = _handler.ReadJwtToken(token);
        if (securityToken is null ||
            !EnumHelper.TryParseFromString<JwtTokenType>(securityToken.Header.Typ, out var tokenType))
        {
            return TokenValidationResult.Fail();
        }

        var validator = _validationProvider.CreateValidator(tokenType.Value);
        return await validator.ValidateAsync(token, cancellationToken);
    }
}