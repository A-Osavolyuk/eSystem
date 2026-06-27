using PeterO.Cbor;

namespace eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement.Apple;

public sealed class AppleAttestationStatementParser : AttestationStatementParser<AppleAttestationStatement>
{
    public override AttestationFormatType FormatType => AttestationFormatType.Apple;
    
    public override AppleAttestationStatement Parse(CBORObject attestationStatement)
    {
        var certs = attestationStatement.ContainsKey("x5c")
            ? attestationStatement["x5c"]
                .Values
                .Select(x => x.GetByteString())
                .ToArray()
            : throw new InvalidOperationException("Missing x5c");

        return new AppleAttestationStatement
        {
            Certificates = certs
        };
    }
}