namespace eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement.Packed;

public sealed class PackedAttestationStatement : IAttestationStatement
{
    public required int Algorithm { get; init; }
    public required byte[] Signature { get; init; }
    public byte[][] Certificates { get; init; } = [];
    public byte[]? EcdaaKeyId { get; init; }
}