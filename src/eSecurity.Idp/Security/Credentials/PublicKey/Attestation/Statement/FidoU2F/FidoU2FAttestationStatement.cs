namespace eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement.FidoU2F;

public sealed class FidoU2FAttestationStatement : IAttestationStatement
{
    public required byte[] Signature { get; init; }
    public required byte[][] Certificates { get; init; }
}