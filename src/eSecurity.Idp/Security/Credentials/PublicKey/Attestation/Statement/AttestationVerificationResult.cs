using System.Security.Cryptography.X509Certificates;

namespace eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement;

public sealed class AttestationVerificationResult
{
    public required bool IsValid { get; init; }

    public AttestationTrustType TrustType { get; init; }

    public X509Certificate2? AttestationCertificate { get; init; }

    public static AttestationVerificationResult Fail()
    {
        return new AttestationVerificationResult
        {
            IsValid = false,
            TrustType = AttestationTrustType.Untrusted
        };
    }
}