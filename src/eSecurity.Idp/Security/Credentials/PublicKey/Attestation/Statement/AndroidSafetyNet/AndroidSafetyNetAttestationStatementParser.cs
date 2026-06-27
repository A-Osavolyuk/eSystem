using PeterO.Cbor;

namespace eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement.AndroidSafetyNet;

public sealed class AndroidSafetyNetAttestationStatementParser 
    : AttestationStatementParser<AndroidSafetyNetAttestationStatement>
{
    public override AttestationFormatType FormatType => AttestationFormatType.AndroidSafetyNet;
    
    public override AndroidSafetyNetAttestationStatement Parse(CBORObject attestationStatement)
    {
        if (!attestationStatement.ContainsKey("response"))
            throw new InvalidOperationException("Missing response");

        return new AndroidSafetyNetAttestationStatement
        {
            Response = attestationStatement["response"].AsString()
        };
    }
}