using System.Formats.Asn1;
using System.Security.Cryptography;

namespace eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement.Tpm;

public sealed class TpmPublicKey
{
    public RSAParameters? Rsa { get; init; }
    public ECParameters? Ec { get; init; }
    public bool IsRsa => Rsa.HasValue;
    public bool IsEc => Ec.HasValue;
}