using System.Security.Claims;
using eSecurity.Security.Cryptography.Keys.PrivateKey;

namespace eSecurity.Security.Cryptography.Tokens.Jwt;

public class JwtTokenFactory(
    IJwtSigner signer,
    IKeyProvider keyProvider) : ITokenFactory
{
    private readonly IJwtSigner signer = signer;
    private readonly IKeyProvider keyProvider = keyProvider;

    public async Task<string> CreateAsync(IEnumerable<Claim> claims)
    {
        var privateKey = await keyProvider.GetPrivateKeyAsync();
        if (privateKey is null) throw new NullReferenceException("Private key is null.");

        var securityKey = new RsaSecurityKey(privateKey) { KeyId = Guid.CreateVersion7().ToString() };
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);
        return signer.Sign(claims, signingCredentials);
    }
}