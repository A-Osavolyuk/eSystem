using System.Security.Claims;
using System.Text.Json;
using eSecurity.Idp.Security.Authentication.Client;
using eSecurity.Idp.Security.Cryptography.Hashing;
using eSecurity.Idp.Security.Cryptography.Tokens;
using eSystem.Core.Security.Identity.Claims;
using eSystem.Core.Server.Security.Authorization.OAuth.Token.Validation;
using TokenValidationResult = eSystem.Core.Server.Security.Authorization.OAuth.Token.Validation.TokenValidationResult;

namespace eSecurity.Idp.Security.Authorization.Token.Validation;

public class OpaqueTokenValidator(
    ITokenQueryService tokenQueryService,
    IClientQueryService clientQueryService,
    IOptions<TokenConfigurations> options) : ITokenValidator
{
    private readonly ITokenQueryService _tokenQueryService = tokenQueryService;
    private readonly IClientQueryService _clientQueryService = clientQueryService;
    private readonly TokenConfigurations _tokenConfigurations = options.Value;

    public async Task<TokenValidationResult> ValidateAsync(string token, CancellationToken cancellationToken = default)
    {
        if (!await _tokenQueryService.ExistsAsync(token, cancellationToken))
            return TokenValidationResult.Fail();

        var opaqueToken = await _tokenQueryService.GetByTokenAsync(token, cancellationToken);
        if (opaqueToken is null || !opaqueToken.IsValid)
            return TokenValidationResult.Fail();

        var audiences = await _clientQueryService.GetSupportedAudiencesAsync(opaqueToken.ClientId, cancellationToken);
        var audClaimValue = JsonSerializer.Serialize(audiences.Select(x => x.Audience));
        var scopes = opaqueToken.Scopes.Select(x => x.ClientScope);
        var claims = new List<Claim>
        {
            new(AppClaimTypes.Jti, opaqueToken.Id.ToString()),
            new(AppClaimTypes.Sid, opaqueToken.SessionId!.Value.ToString()),
            new(AppClaimTypes.Aud, audClaimValue),
            new(AppClaimTypes.Iss, _tokenConfigurations.Issuer),
            new(AppClaimTypes.Sub, opaqueToken.Subject),
            new(AppClaimTypes.Iat, opaqueToken.IssuedAt.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new(AppClaimTypes.Exp, opaqueToken.ExpiredAt.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new(AppClaimTypes.Scope, string.Join(" ", scopes)),
        };

        if (opaqueToken.NotBefore.HasValue)
        {
            var nbf = opaqueToken.NotBefore.Value.ToUnixTimeSeconds().ToString();
            claims.Add(new Claim(AppClaimTypes.Nbf, nbf, ClaimValueTypes.Integer64));
        }
        
        if (opaqueToken.SessionId.HasValue)
            claims.Add(new Claim(AppClaimTypes.Nbf, opaqueToken.SessionId.Value.ToString()));

        var claimsIdentity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        
        return TokenValidationResult.Success(claimsPrincipal);
    }
}