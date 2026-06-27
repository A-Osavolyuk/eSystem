using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement.FidoU2F;

public sealed class FidoU2FAttestationStatementVerifier : AttestationStatementVerifier<FidoU2FAttestationStatement>
{
    public override AttestationFormatType Format => AttestationFormatType.FidoU2F;

    public override async ValueTask<AttestationVerificationResult> VerifyAsync(
        AuthenticationData authenticationData, 
        FidoU2FAttestationStatement attestationStatement,
        byte[] clientDataHash, 
        CancellationToken cancellationToken = default)
    {
        var rpIdHash = authenticationData.Raw.AsSpan(0, 32).ToArray();
        
        var signedData = new byte[] { 0x00 }
                .Concat(rpIdHash)
                .Concat(clientDataHash)
                .ToArray();

        var cert = X509CertificateLoader.LoadCertificate(attestationStatement.Certificates[0]);

        using var ecdsa = cert.GetECDsaPublicKey();
        if (ecdsa is null)
            throw new InvalidOperationException("Invalid ECDSA");

        var valid = ecdsa.VerifyData(
            signedData,
            attestationStatement.Signature,
            HashAlgorithmName.SHA256);

        return new AttestationVerificationResult
        {
            IsValid = valid,
            TrustType = AttestationTrustType.Untrusted,
            AttestationCertificate = cert
        };
    }
}