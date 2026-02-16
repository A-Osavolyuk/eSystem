using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using eSecurity.Server.Security.Authorization.OAuth.Token.Validation;
using eSecurity.Server.Security.Cryptography.Tokens;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authorization.OAuth.Constants;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange.Delegation;

public sealed class JwtTokenClaimsExtractor(
    IJwtTokenValidationProvider validationProvider,
    IOptions<TokenConfigurations> options) : ITokenClaimsExtractor
{
    private readonly IJwtTokenValidationProvider _validationProvider = validationProvider;
    private readonly TokenConfigurations _configurations = options.Value;
    private readonly JwtSecurityTokenHandler _handler = new();

    public async ValueTask<TypedResult<IEnumerable<Claim>>> ExtractAsync(string subjectToken, CancellationToken cancellationToken)
    {
        if (!_handler.CanReadToken(subjectToken))
        {
            return TypedResult<IEnumerable<Claim>>.Fail(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidToken,
                Description = "Invalid subject token"
            });
        }
        
        var securityToken = _handler.ReadJwtToken(subjectToken);
        if (securityToken is null ||
            !securityToken.Header.Typ.Equals(JwtTokenTypes.AccessToken, StringComparison.OrdinalIgnoreCase))
        {
            return TypedResult<IEnumerable<Claim>>.Fail(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidToken,
                Description = "Invalid subject token"
            });
        }

        var validator = _validationProvider.CreateValidator(securityToken.Header.Typ);
        var validationResult = await validator.ValidateAsync(subjectToken, cancellationToken);
        if (!validationResult.IsValid || validationResult.ClaimsPrincipal is null)
        {
            
            return TypedResult<IEnumerable<Claim>>.Fail(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidToken,
                Description = "Invalid subject token"
            });
        }

        var tokenClaims = validationResult.ClaimsPrincipal.Claims.ToList();
        var extractedClaims = new List<Claim>
        {
            new (AppClaimTypes.Iss, _configurations.Issuer),
            new (AppClaimTypes.Jti, Guid.CreateVersion7().ToString()),
        };
        
        var iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        var iatClaim = new Claim(AppClaimTypes.Iat, iat, ClaimValueTypes.Integer64);
        extractedClaims.Add(iatClaim);
        
        var exp = DateTimeOffset.UtcNow.Add(_configurations.DefaultAccessTokenLifetime).ToUnixTimeSeconds().ToString();
        var expClaim = new Claim(AppClaimTypes.Exp, exp, ClaimValueTypes.Integer64);
        extractedClaims.Add(expClaim);
        
        var subClaim = tokenClaims.FirstOrDefault(x => x.Type == AppClaimTypes.Sub);
        if (subClaim is not null)
            extractedClaims.Add(subClaim);

        var scopeClaim = tokenClaims.FirstOrDefault(x => x.Type == AppClaimTypes.Scope);
        if (scopeClaim is not null)
            extractedClaims.Add(scopeClaim);
        
        var clientIdClaim = tokenClaims.FirstOrDefault(x => x.Type == AppClaimTypes.ClientId);
        if (clientIdClaim is not null)
            extractedClaims.Add(clientIdClaim);
        
        var actClaim = tokenClaims.FirstOrDefault(x => x.Type == AppClaimTypes.Act);
        if (actClaim is not null)
            extractedClaims.Add(actClaim);
        
        var delegatedClaim = tokenClaims.FirstOrDefault(x => x.Type == AppClaimTypes.Delegated);
        if (delegatedClaim is not null)
            extractedClaims.Add(delegatedClaim);
        
        return TypedResult<IEnumerable<Claim>>.Success(extractedClaims);
    }
}