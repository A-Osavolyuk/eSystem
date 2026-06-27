namespace eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement;

public interface IAttestationStatementParserProvider
{
    IAttestationStatementParser GetParser(AttestationFormatType formatType);
}