using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using eSecurity.Server.Security.Authorization.OAuth.Token.Validation;
using eSystem.Core.Security.Authorization.OAuth.Constants;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange.Claims;

public sealed class JwtTokenClaimsExtractor(
    IJwtTokenValidationProvider validationProvider,
    IOptions<TokenOptions> options) : ITokenClaimsExtractor
{
    private readonly IJwtTokenValidationProvider _validationProvider = validationProvider;
    private readonly TokenOptions _options = options.Value;
    private readonly JwtSecurityTokenHandler _handler = new();

    public async ValueTask<ClaimExtractionResult> ExtractAsync(string subjectToken, CancellationToken cancellationToken)
    {
        if (!_handler.CanReadToken(subjectToken))
            return ClaimExtractionResult.Fail();
        
        var securityToken = _handler.ReadJwtToken(subjectToken);
        if (securityToken is null ||
            !securityToken.Header.Typ.Equals(JwtTokenTypes.AccessToken, StringComparison.OrdinalIgnoreCase))
        {
            return ClaimExtractionResult.Fail();
        }

        var validator = _validationProvider.CreateValidator(securityToken.Header.Typ);
        var validationResult = await validator.ValidateAsync(subjectToken, cancellationToken);
        if (!validationResult.IsValid || validationResult.ClaimsPrincipal is null) 
            return ClaimExtractionResult.Fail();

        var tokenClaims = validationResult.ClaimsPrincipal.Claims.ToList();
        var extractedClaims = new List<Claim>
        {
            new (AppClaimTypes.Iss, _options.Issuer),
            new (AppClaimTypes.Jti, Guid.CreateVersion7().ToString()),
        };
        
        var iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        var iatClaim = new Claim(AppClaimTypes.Iat, iat, ClaimValueTypes.Integer64);
        extractedClaims.Add(iatClaim);
        
        var exp = DateTimeOffset.UtcNow.Add(_options.AccessTokenLifetime).ToUnixTimeSeconds().ToString();
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
        
        return ClaimExtractionResult.Success(extractedClaims);
    }
}