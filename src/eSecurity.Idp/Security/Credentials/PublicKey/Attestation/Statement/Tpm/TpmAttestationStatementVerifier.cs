using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using eSecurity.Idp.Security.Cryptography.Certificates;

namespace eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement.Tpm;

public sealed class TpmAttestationStatementVerifier
    : AttestationStatementVerifier<TpmAttestationStatement>
{
    public override AttestationFormatType Format => AttestationFormatType.Tpm;

    public override async ValueTask<AttestationVerificationResult> VerifyAsync(
        AuthenticationData authenticationData,
        TpmAttestationStatement attestationStatement,
        byte[] clientDataHash,
        CancellationToken cancellationToken = default)
    {
        var cert = X509CertificateLoader.LoadCertificate(attestationStatement.Certificates[0]);

        var tpm = TpmAttestationParser.Parse(attestationStatement.CertInfo);

        // 1. nonce binding check
        if (!tpm.ExtraData.SequenceEqual(clientDataHash))
            return AttestationVerificationResult.Fail();

        // 2. signature verification (correct data = TPM attestation structure)
        if (!VerifySignature(cert, tpm.Raw, attestationStatement.Signature))
            return AttestationVerificationResult.Fail();

        // 3. pubArea must match certificate public key (CRITICAL FIX)
        if (!TpmPubAreaVerifier.Verify(tpm.PubArea, cert))
            return AttestationVerificationResult.Fail();

        // 4. certificate trust chain
        if (!CertificateTrustValidator.Validate(cert))
            return AttestationVerificationResult.Fail();

        return new AttestationVerificationResult
        {
            IsValid = true,
            TrustType = AttestationTrustType.Trusted,
            AttestationCertificate = cert
        };
    }

    private static bool VerifySignature(
        X509Certificate2 cert,
        byte[] data,
        byte[] signature)
    {
        using var ecdsa = cert.GetECDsaPublicKey();
        if (ecdsa != null)
            return ecdsa.VerifyData(data, signature, HashAlgorithmName.SHA256);

        using var rsa = cert.GetRSAPublicKey();
        if (rsa != null)
        {
            return rsa.VerifyData(
                data,
                signature,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);
        }

        return false;
    }
}