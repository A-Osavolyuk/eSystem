using PeterO.Cbor;

namespace eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement.Tpm;

public sealed class TpmAttestationStatementParser : AttestationStatementParser<TpmAttestationStatement>
{
    public override AttestationFormatType FormatType => AttestationFormatType.Tpm;

    public override TpmAttestationStatement Parse(CBORObject attestationStatement)
    {
        return new TpmAttestationStatement
        {
            Version = attestationStatement["ver"].AsString(),
            Algorithm = attestationStatement["alg"].AsInt32(),
            Signature = attestationStatement["sig"].GetByteString(),
            CertInfo = attestationStatement["certInfo"].GetByteString(),
            PubArea = attestationStatement["pubArea"].GetByteString(),
            Certificates = attestationStatement.ContainsKey("x5c")
                ? attestationStatement["x5c"].Values.Select(x => x.GetByteString()).ToArray()
                : []
        };
    }
}