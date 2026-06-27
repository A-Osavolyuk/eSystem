namespace eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement;

public interface IAttestationStatementVerifierProvider
{
    IAttestationStatementVerifier GetVerifier(AttestationFormatType formatType);
}