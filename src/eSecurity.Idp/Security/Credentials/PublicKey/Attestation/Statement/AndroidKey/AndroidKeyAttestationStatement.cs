namespace eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement.AndroidKey;

public sealed class AndroidKeyAttestationStatement : IAttestationStatement
{
    public required byte[] Signature { get; init; }
    public required byte[][] Certificates { get; init; }
}