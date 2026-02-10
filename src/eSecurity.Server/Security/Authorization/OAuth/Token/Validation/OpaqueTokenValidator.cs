using System.Security.Claims;
using System.Text.Json;
using eSecurity.Server.Security.Cryptography.Hashing;
using eSystem.Core.Security.Authorization.OAuth.Token.Validation;
using eSystem.Core.Security.Identity.Claims;
using TokenValidationResult = eSystem.Core.Security.Authorization.OAuth.Token.Validation.TokenValidationResult;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.Validation;

public class OpaqueTokenValidator(
    ITokenManager tokenManager,
    IHasherProvider hasherProvider,
    IOptions<TokenOptions> options) : ITokenValidator
{
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly IHasher _hasher = hasherProvider.GetHasher(HashAlgorithm.Sha512);
    private readonly TokenOptions _tokenConfigurations = options.Value;

    public async Task<TokenValidationResult> ValidateAsync(string token, CancellationToken cancellationToken = default)
    {
        if (!await _tokenManager.IsOpaqueAsync(token, cancellationToken))
            return TokenValidationResult.Fail();
        
        var incomingHash = _hasher.Hash(token);
        var opaqueToken = await _tokenManager.FindByHashAsync(incomingHash, cancellationToken);
        if (opaqueToken is null || !opaqueToken.IsValid)
            return TokenValidationResult.Fail();

        var aud = JsonSerializer.Serialize(opaqueToken.Client.Audiences.Select(x => x.Audience));
        var scopes = opaqueToken.Scopes.Select(x => x.ClientScope);
        var claims = new List<Claim>
        {
            new(AppClaimTypes.Jti, opaqueToken.Id.ToString()),
            new(AppClaimTypes.Sid, opaqueToken.SessionId!.Value.ToString()),
            new(AppClaimTypes.Aud, aud),
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