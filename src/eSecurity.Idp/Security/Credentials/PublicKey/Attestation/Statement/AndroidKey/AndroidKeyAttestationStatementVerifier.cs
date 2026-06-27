using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement.AndroidKey;

public sealed class AndroidKeyAttestationStatementVerifier
    : AttestationStatementVerifier<AndroidKeyAttestationStatement>
{
    public override AttestationFormatType Format => AttestationFormatType.AndroidKey;
    private const string OidExtension = "1.3.6.1.4.1.11129.2.1.17";

    public override async ValueTask<AttestationVerificationResult> VerifyAsync(
        AuthenticationData authenticationData,
        AndroidKeyAttestationStatement attestationStatement,
        byte[] clientDataHash,
        CancellationToken cancellationToken = default)
    {
        var signedData = authenticationData.Raw
            .Concat(clientDataHash)
            .ToArray();

        var cert = X509CertificateLoader.LoadCertificate(
            attestationStatement.Certificates[0]);
        
        var chain = new X509Chain
        {
            ChainPolicy =
            {
                RevocationMode = X509RevocationMode.NoCheck,
                VerificationFlags = X509VerificationFlags.NoFlag,
                VerificationTime = DateTime.UtcNow
            }
        };
        
        if (!chain.Build(cert))
            return AttestationVerificationResult.Fail();
        
        using var ecdsa = cert.GetECDsaPublicKey();
        if (ecdsa is null)
            return AttestationVerificationResult.Fail();

        var valid = ecdsa.VerifyData(
            signedData,
            attestationStatement.Signature,
            HashAlgorithmName.SHA256);

        if (!valid)
            return AttestationVerificationResult.Fail();
        
        var extension = cert.Extensions.FirstOrDefault(e => e.Oid?.Value == OidExtension);
        if (extension is null)
            return AttestationVerificationResult.Fail();

        return new AttestationVerificationResult
        {
            IsValid = true,
            TrustType = AttestationTrustType.Untrusted,
            AttestationCertificate = cert
        };
    }
}