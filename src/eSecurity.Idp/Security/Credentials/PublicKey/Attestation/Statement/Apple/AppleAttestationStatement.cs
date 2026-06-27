namespace eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement.Apple;

public sealed class AppleAttestationStatement : IAttestationStatement
{
    public required byte[][] Certificates { get; init; }
}