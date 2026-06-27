using PeterO.Cbor;

namespace eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement.None;

public sealed class NoneAttestationStatementParser : AttestationStatementParser<NoneAttestationStatement>
{
    public override AttestationFormatType FormatType => AttestationFormatType.None;
    
    public override NoneAttestationStatement Parse(CBORObject attestationStatement)
    {
        return new NoneAttestationStatement();
    }
}