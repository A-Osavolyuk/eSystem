using System.Security.Claims;
using eSecurity.Security.Cryptography.Keys.SigningKey;

namespace eSecurity.Security.Cryptography.Tokens.Jwt;

public class JwtTokenFactory(
    IJwtSigner signer,
    ISigningKeyProvider signingKeyProvider) : ITokenFactory
{
    private readonly IJwtSigner signer = signer;
    private readonly ISigningKeyProvider signingKeyProvider = signingKeyProvider;

    public async Task<string> CreateAsync(IEnumerable<Claim> claims)
    {
        var signingKey = await signingKeyProvider.GetAsync();
        if (signingKey is null) throw new NullReferenceException("Private key is null.");

        var securityKey = new RsaSecurityKey(signingKey.PrivateKey) { KeyId = signingKey.Id.ToString() };
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);
        return signer.Sign(claims, signingCredentials);
    }
}