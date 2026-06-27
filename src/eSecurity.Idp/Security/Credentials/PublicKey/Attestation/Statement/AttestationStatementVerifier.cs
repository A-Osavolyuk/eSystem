namespace eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement;

public abstract class AttestationStatementVerifier<TStatement> : IAttestationStatementVerifier<TStatement> 
    where TStatement : IAttestationStatement
{
    public abstract AttestationFormatType Format { get; }

    public abstract ValueTask<AttestationVerificationResult> VerifyAsync(
        AuthenticationData authenticationData,
        TStatement attestationStatement, 
        byte[] clientDataHash,
        CancellationToken cancellationToken = default);

    public virtual async ValueTask<AttestationVerificationResult> VerifyAsync(
        AuthenticationData authenticationData, 
        IAttestationStatement attestationStatement,
        byte[] clientDataHash, 
        CancellationToken cancellationToken = default)
    {
        if (attestationStatement is not TStatement)
            throw new InvalidOperationException("Invalid attestation statement");

        return await VerifyAsync(authenticationData, attestationStatement, clientDataHash, cancellationToken);
    }
}