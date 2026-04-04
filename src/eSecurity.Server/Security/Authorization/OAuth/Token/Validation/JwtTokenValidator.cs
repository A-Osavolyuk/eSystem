using System.IdentityModel.Tokens.Jwt;
using eSystem.Core.Enums;
using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Security.Authorization.OAuth.Constants;
using eSystem.Core.Security.Authorization.OAuth.Token.Validation;
using TokenValidationResult = eSystem.Core.Security.Authorization.OAuth.Token.Validation.TokenValidationResult;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.Validation;

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

        var tokenType = EnumHelper.FromStringOrThrow<JwtTokenType>(securityToken.Header.Typ);
        var validator = _validationProvider.CreateValidator(tokenType);
        return await validator.ValidateAsync(token, cancellationToken);
    }
}