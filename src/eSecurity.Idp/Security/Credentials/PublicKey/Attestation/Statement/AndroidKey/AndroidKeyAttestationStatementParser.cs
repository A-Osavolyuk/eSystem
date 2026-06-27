using PeterO.Cbor;

namespace eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement.AndroidKey;

public sealed class AndroidKeyAttestationStatementParser : AttestationStatementParser<AndroidKeyAttestationStatement>
{
    public override AttestationFormatType FormatType => AttestationFormatType.AndroidKey;
    
    public override AndroidKeyAttestationStatement Parse(CBORObject attestationStatement)
    {
        var sig = attestationStatement["sig"].GetByteString();

        var certs = attestationStatement.ContainsKey("x5c")
            ? attestationStatement["x5c"].Values.Select(x => x.GetByteString()).ToArray()
            : [];

        return new AndroidKeyAttestationStatement
        {
            Signature = sig,
            Certificates = certs
        };
    }
}