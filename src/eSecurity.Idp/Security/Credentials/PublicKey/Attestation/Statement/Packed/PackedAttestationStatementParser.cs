using PeterO.Cbor;

namespace eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement.Packed;

public sealed class PackedAttestationStatementParser : AttestationStatementParser<PackedAttestationStatement>
{
    public override AttestationFormatType FormatType => AttestationFormatType.Packed;

    public override PackedAttestationStatement Parse(CBORObject attestationStatement)
    {
        var sig = attestationStatement.ContainsKey("sig")
            ? attestationStatement["sig"].GetByteString()
            : throw new InvalidOperationException("Missing sig");

        var alg = attestationStatement.ContainsKey("alg")
            ? attestationStatement["alg"].AsInt32()
            : throw new InvalidOperationException("Missing alg");

        var certs = attestationStatement.ContainsKey("x5c")
            ? attestationStatement["x5c"]
                .Values
                .Select(x => x.GetByteString())
                .ToArray()
            : [];

        var ecdaa = attestationStatement.ContainsKey("ecdaaKeyId")
            ? attestationStatement["ecdaaKeyId"].GetByteString()
            : null;

        return new PackedAttestationStatement
        {
            Algorithm = alg,
            Signature = sig,
            Certificates = certs,
            EcdaaKeyId = ecdaa
        };
    }
}