namespace eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement;

public interface IAttestationStatementVerifier<in TStatement> : IAttestationStatementVerifier
    where TStatement : IAttestationStatement
{
    ValueTask<AttestationVerificationResult> VerifyAsync(
        AuthenticationData authenticationData,
        TStatement attestationStatement,
        byte[] clientDataHash,
        CancellationToken cancellationToken = default);
}

public interface IAttestationStatementVerifier
{
    public AttestationFormatType Format { get; }
    
    ValueTask<AttestationVerificationResult> VerifyAsync(
        AuthenticationData authenticationData,
        IAttestationStatement attestationStatement,
        byte[] clientDataHash,
        CancellationToken cancellationToken = default);
}