using PeterO.Cbor;

namespace eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement.FidoU2F;

public sealed class FidoU2FAttestationStatementParser : AttestationStatementParser<FidoU2FAttestationStatement>
{
    public override AttestationFormatType FormatType => AttestationFormatType.FidoU2F;
    
    public override FidoU2FAttestationStatement Parse(CBORObject attestationStatement)
    {
        var sig = attestationStatement.ContainsKey("sig")
            ? attestationStatement["sig"].GetByteString()
            : throw new InvalidOperationException("Missing sig");

        var certs = attestationStatement.ContainsKey("x5c")
            ? attestationStatement["x5c"]
                .Values
                .Select(x => x.GetByteString())
                .ToArray()
            : throw new InvalidOperationException("Missing x5c");

        return new FidoU2FAttestationStatement
        {
            Signature = sig,
            Certificates = certs
        };
    }
}