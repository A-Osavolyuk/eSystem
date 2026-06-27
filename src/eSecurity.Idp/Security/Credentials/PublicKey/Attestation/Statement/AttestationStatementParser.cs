using PeterO.Cbor;

namespace eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement;

public abstract class AttestationStatementParser<TStatement> : IAttestationStatementParser<TStatement> 
    where TStatement : IAttestationStatement
{
    public abstract AttestationFormatType FormatType { get; }

    public abstract TStatement Parse(CBORObject attestationStatement);

    IAttestationStatement IAttestationStatementParser.Parse(CBORObject attestationStatement)
    {
        return Parse(attestationStatement);
    }
}