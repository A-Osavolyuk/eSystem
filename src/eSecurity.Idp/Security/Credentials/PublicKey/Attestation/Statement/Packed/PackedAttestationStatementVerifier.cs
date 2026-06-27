using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement.Packed;

public sealed class PackedAttestationStatementVerifier : AttestationStatementVerifier<PackedAttestationStatement>
{
    public override AttestationFormatType Format => AttestationFormatType.Packed;

    public override async ValueTask<AttestationVerificationResult> VerifyAsync(
        AuthenticationData authenticationData, 
        PackedAttestationStatement attestationStatement,
        byte[] clientDataHash, 
        CancellationToken cancellationToken = default)
    {
        var data = authenticationData
            .Raw
            .Concat(clientDataHash)
            .ToArray();
        
        var cert = X509CertificateLoader.LoadCertificate(attestationStatement.Certificates[0]);
        
        using var ecdsa = cert.GetECDsaPublicKey();
        if (ecdsa is null)
            throw new InvalidOperationException("Invalid ECDSA");

        var valid = ecdsa.VerifyData(
            data,
            attestationStatement.Signature,
            HashAlgorithmName.SHA256);

        if (!valid)
            return AttestationVerificationResult.Fail();

        return new AttestationVerificationResult
        {
            IsValid = true,
            AttestationCertificate = cert,
            TrustType = AttestationTrustType.Untrusted
        };
    }
}