using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using eSecurity.Server.Security.Cryptography.Signing;
using eSecurity.Server.Security.Cryptography.Signing.Certificates;

namespace eSecurity.Server.Security.Cryptography.Tokens;

public sealed class JwtTokenBuildContext : TokenBuildContext
{
    public required IEnumerable<Claim> Claims { get; set; }
    public required string Type { get; set; }
}

public class JwtTokenBuilder(
    IJwtSigner signer,
    ICertificateProvider certificateProvider) : ITokenBuilder<JwtTokenBuildContext, string>
{
    private readonly IJwtSigner _signer = signer;
    private readonly ICertificateProvider _certificateProvider = certificateProvider;

    public async ValueTask<string> BuildAsync(JwtTokenBuildContext buildContext, CancellationToken cancellationToken = default)
    {
        var certificate = await _certificateProvider.GetActiveAsync(cancellationToken);
        var privateKey = certificate.Certificate.GetRSAPrivateKey();
        if (privateKey is null) throw new NullReferenceException("Private key is null.");

        var securityKey = new RsaSecurityKey(privateKey) { KeyId = certificate.Id.ToString() };
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);
        return _signer.Sign(buildContext.Claims, signingCredentials, buildContext.Type);
    }
}