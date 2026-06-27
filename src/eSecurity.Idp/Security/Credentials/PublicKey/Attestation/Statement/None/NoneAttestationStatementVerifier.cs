namespace eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement.None;

public sealed class NoneAttestationStatementVerifier : AttestationStatementVerifier<NoneAttestationStatement>
{
    public override AttestationFormatType Format => AttestationFormatType.None;

    public override async ValueTask<AttestationVerificationResult> VerifyAsync(
        AuthenticationData authenticationData, 
        NoneAttestationStatement attestationStatement,
        byte[] clientDataHash, 
        CancellationToken cancellationToken = default)
    {
        return new AttestationVerificationResult
        {
            IsValid = true,
            TrustType = AttestationTrustType.None
        };
    }
}