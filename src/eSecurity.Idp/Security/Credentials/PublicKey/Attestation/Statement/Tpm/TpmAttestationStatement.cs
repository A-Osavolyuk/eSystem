namespace eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement.Tpm;

public sealed class TpmAttestationStatement : IAttestationStatement
{
    public required string Version { get; init; }
    public required int Algorithm { get; init; }
    public required byte[] Signature { get; init; }
    public required byte[] CertInfo { get; init; }
    public required byte[] PubArea { get; init; }
    public byte[][] Certificates { get; init; } = [];
}