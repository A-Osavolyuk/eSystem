using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using eSecurity.Security.Cryptography.Signing.Certificates;

namespace eSecurity.Security.Cryptography.Tokens.Jwt;

public class JwtTokenFactory(
    IJwtSigner signer,
    ICertificateProvider certificateProvider) : ITokenFactory
{
    private readonly IJwtSigner signer = signer;
    private readonly ICertificateProvider certificateProvider = certificateProvider;

    public async Task<string> CreateAsync(IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
    {
        var certificate = await certificateProvider.GetCertificateAsync(cancellationToken);
        var privateKey = certificate.Certificate.GetRSAPrivateKey();
        if (privateKey is null) throw new NullReferenceException("Private key is null.");

        var securityKey = new RsaSecurityKey(privateKey) { KeyId = certificate.Id.ToString() };
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);
        return signer.Sign(claims, signingCredentials);
    }
}