using PeterO.Cbor;

namespace eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement;

public interface IAttestationStatementParser<out TStatement> : IAttestationStatementParser
    where TStatement : IAttestationStatement
{
    new TStatement Parse(CBORObject attestationStatement);
}

public interface IAttestationStatementParser
{
    public AttestationFormatType FormatType { get; }
    
    IAttestationStatement Parse(CBORObject attestationStatement);
}