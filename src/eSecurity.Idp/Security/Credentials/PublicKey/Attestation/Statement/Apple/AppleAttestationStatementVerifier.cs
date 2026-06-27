using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement.Apple;

public sealed class AppleAttestationStatementVerifier
    : AttestationStatementVerifier<AppleAttestationStatement>
{
    public override AttestationFormatType Format => AttestationFormatType.Apple;
    private const string OidExtension = "1.2.840.113635.100.8.2";

    public override async ValueTask<AttestationVerificationResult> VerifyAsync(
        AuthenticationData authenticationData,
        AppleAttestationStatement attestationStatement,
        byte[] clientDataHash,
        CancellationToken cancellationToken = default)
    {
        var cert = X509CertificateLoader.LoadCertificate(
            attestationStatement.Certificates[0]);
        
        var chain = new X509Chain
        {
            ChainPolicy =
            {
                RevocationMode = X509RevocationMode.NoCheck
            }
        };

        if (!chain.Build(cert))
            return AttestationVerificationResult.Fail();
        
        var nonce = SHA256.HashData(authenticationData.Raw.Concat(clientDataHash).ToArray());
        var ext = cert.Extensions.FirstOrDefault(e => e.Oid?.Value == OidExtension);
        if (ext is null)
            throw new InvalidOperationException("Missing Apple extension");
        
        var certNonce = ext.RawData;
        if (!certNonce.SequenceEqual(nonce))
            return AttestationVerificationResult.Fail();

        return new AttestationVerificationResult
        {
            IsValid = true,
            TrustType = AttestationTrustType.Untrusted,
            AttestationCertificate = cert
        };
    }
}