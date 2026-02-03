using System.Security.Claims;
using eSecurity.Server.Security.Cryptography.Hashing;
using eSystem.Core.Security.Authorization.OAuth.Token.Validation;
using eSystem.Core.Security.Identity.Claims;
using TokenValidationResult = eSystem.Core.Security.Authorization.OAuth.Token.Validation.TokenValidationResult;

namespace eSecurity.Server.Security.Authorization.Token.Validation;

public class OpaqueTokenValidator(
    ITokenManager tokenManager,
    IHasherProvider hasherProvider,
    IOptions<TokenOptions> options) : ITokenValidator
{
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly IHasher _hasher = hasherProvider.GetHasher(HashAlgorithm.Sha512);
    private readonly TokenOptions _tokenOptions = options.Value;

    public async Task<TokenValidationResult> ValidateAsync(string token, CancellationToken cancellationToken = default)
    {
        if (!await _tokenManager.IsOpaqueAsync(token, cancellationToken))
            return TokenValidationResult.Fail();
        
        var incomingHash = _hasher.Hash(token);
        var opaqueToken = await _tokenManager.FindByHashAsync(incomingHash, cancellationToken);
        if (opaqueToken is null || !opaqueToken.IsValid)
            return TokenValidationResult.Fail();

        var scopes = opaqueToken.Scopes.Select(x => x.Scope);
        var claims = new List<Claim>
        {
            new(AppClaimTypes.Jti, opaqueToken.Id.ToString()),
            new(AppClaimTypes.Sid, opaqueToken.SessionId!.Value.ToString()),
            new(AppClaimTypes.Aud, opaqueToken.Client.Audience),
            new(AppClaimTypes.Iss, _tokenOptions.Issuer),
            new(AppClaimTypes.Sub, opaqueToken.Session!.UserId.ToString()),
            new(AppClaimTypes.Iat, opaqueToken.CreateDate!.Value.ToUnixTimeSeconds().ToString()),
            new(AppClaimTypes.Exp, opaqueToken.ExpiredDate.ToUnixTimeSeconds().ToString()),
            new(AppClaimTypes.Scope, string.Join(" ", scopes)),
        };

        var claimsIdentity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        
        return TokenValidationResult.Success(claimsPrincipal);
    }
}